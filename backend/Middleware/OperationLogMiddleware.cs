using System.Text.Json;
using Dapper;
using Letiao.ATS.Api.Common;
using MySql.Data.MySqlClient;

namespace Letiao.ATS.Api.Middleware;

/// <summary>
/// 操作日志中间件：在每次 API 请求完成后，将操作轨迹写入 ats_timeline_log
/// 仅记录写操作（POST/PUT/PATCH/DELETE）
/// </summary>
public class OperationLogMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<OperationLogMiddleware> logger)
{
    private static readonly HashSet<string> WriteMethods =
        new(StringComparer.OrdinalIgnoreCase) { "POST", "PUT", "PATCH", "DELETE" };

    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        // 仅对写操作且成功响应进行日志记录（跳过认证端点）
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? "";
        if (!WriteMethods.Contains(method) || path.StartsWith("/api/auth") || path.StartsWith("/health"))
            return;

        try
        {
            // 从 JWT Claims 中读取操作人 ID
            var operatorIdClaim = context.User.FindFirst("uid")?.Value;
            int? operatorId = operatorIdClaim != null ? int.Parse(operatorIdClaim) : null;

            var content = $"[{method}] {path} → HTTP {context.Response.StatusCode}";

            var connStr = configuration.GetConnectionString("DefaultConnection");
            await using var conn = new MySqlConnection(connStr);

            const string sql = @"
                INSERT INTO ats_timeline_log (candidate_id, operator_id, action_type, content, created_at)
                VALUES (0, @OperatorId, 'SYS_AUTO', @Content, NOW())";

            await conn.ExecuteAsync(sql, new { OperatorId = operatorId, Content = content });
        }
        catch (Exception ex)
        {
            // 日志记录失败不能影响主业务
            logger.LogWarning(ex, "操作日志写入失败，跳过");
        }
    }
}

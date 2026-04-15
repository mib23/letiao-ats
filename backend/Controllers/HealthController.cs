using Letiao.ATS.Api.Common;
using Letiao.ATS.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Letiao.ATS.Api.Controllers;

/// <summary>
/// 健康检查与系统状态接口
/// </summary>
[ApiController]
[Route("api/health")]
public class HealthController(DbConnectionFactory dbFactory) : ControllerBase
{
    /// <summary>
    /// 系统健康检查：验证 API 服务运行正常，并通过 Dapper 探活 MySQL 连接
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Check()
    {
        var dbAlive = await dbFactory.PingAsync();
        var status = new
        {
            Api = "ok",
            Database = dbAlive ? "ok" : "unreachable",
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
        };

        if (!dbAlive)
            return StatusCode(503, ApiResponse.Ok(status, "数据库连接异常"));

        return Ok(ApiResponse.Ok(status, "服务运行正常"));
    }
}

using System.Net;
using System.Text.Json;
using Letiao.ATS.Api.Common;

namespace Letiao.ATS.Api.Middleware;

/// <summary>
/// 全局异常捕获中间件，统一返回 { code, msg, data } 格式
/// </summary>
public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (BusinessException bex)
        {
            logger.LogWarning("业务异常: {Message}", bex.Message);
            await WriteErrorResponseAsync(context, bex.Code, bex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "未处理的全局异常: {Message}", ex.Message);
            await WriteErrorResponseAsync(context, 500, $"服务器内部错误: {ex.Message}");
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, int code, string message)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.StatusCode = code >= 400 && code < 600 ? code : (int)HttpStatusCode.InternalServerError;

        var response = ApiResponse.Fail(code, message);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

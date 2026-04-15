using Letiao.ATS.Api.Application;
using Letiao.ATS.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace Letiao.ATS.Api.Controllers;

/// <summary>
/// 认证接口：登录、登出
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    /// <summary>
    /// 账号密码登录
    /// </summary>
    /// <remarks>
    /// 请求示例：{"username": "admin", "password": "Admin@123"}
    /// </remarks>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(ApiResponse.Fail(400, "用户名和密码不能为空"));

        var result = await authService.LoginAsync(request);
        if (result == null)
            return Unauthorized(ApiResponse.Fail(401, "用户名或密码错误，或账号已禁用"));

        return Ok(ApiResponse.Ok(result, "登录成功"));
    }
}

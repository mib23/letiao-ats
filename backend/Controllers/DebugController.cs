using Dapper;
using Letiao.ATS.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Letiao.ATS.Api.Controllers;

/// <summary>
/// 仅开发环境使用的调试接口（生产时应关闭/移除）
/// </summary>
[ApiController]
[Route("api/debug")]
public class DebugController(DbConnectionFactory dbFactory, IWebHostEnvironment env) : ControllerBase
{
    /// <summary>
    /// 执行数据库初始化种子数据（仅 Development 环境可用）
    /// </summary>
    [HttpPost("seed")]
    public async Task<IActionResult> SeedDatabase()
    {
        if (!env.IsDevelopment())
            return Forbid();

        await using var conn = await dbFactory.CreateOpenConnectionAsync();

        var results = new List<string>();

        // 角色字典
        var roles = new[]
        {
            ("系统管理员", "ROLE_ADMIN"),
            ("招聘HR",     "ROLE_HR"),
            ("用人经理",   "ROLE_BIZ_MGR"),
        };
        foreach (var (name, code) in roles)
        {
            var n = await conn.ExecuteAsync(
                "INSERT IGNORE INTO sys_role (role_name, role_code) VALUES (@Name, @Code)",
                new { Name = name, Code = code });
            if (n > 0) results.Add($"角色 [{code}] 已插入");
        }

        // 管理员账号（密码 Admin@123 的 SHA256）
        const string adminHash = "e86f78a8a3caf0b60d8e74e5942aa6d86dc150cd3c03338aef25b7d2d7e3acc7";
        var userN = await conn.ExecuteAsync(@"
            INSERT IGNORE INTO sys_user (username, password_hash, real_name, department_name, status)
            VALUES ('admin', @Hash, '系统管理员', '信息技术部', 1)",
            new { Hash = adminHash });
        if (userN > 0) results.Add("账号 [admin] 已创建");

        // 绑定管理员角色
        var bindN = await conn.ExecuteAsync(@"
            INSERT IGNORE INTO sys_user_role (user_id, role_id)
            SELECT u.id, r.id FROM sys_user u, sys_role r
            WHERE u.username = 'admin' AND r.role_code = 'ROLE_ADMIN'");
        if (bindN > 0) results.Add("角色绑定完成");

        return Ok(new { msg = "种子数据执行完毕", details = results });
    }
}

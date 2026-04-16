using Dapper;
using Letiao.ATS.Api.Domain.Entities;
using Letiao.ATS.Api.Infrastructure;

namespace Letiao.ATS.Api.Repository;

/// <summary>
/// 系统用户数据仓储
/// </summary>
public class UserRepository(DbConnectionFactory dbFactory, ILogger<UserRepository> logger)
{
    /// <summary>根据用户名查找用户（含角色列表），拆分为两次查询避免多映射问题</summary>
    public async Task<SysUser?> FindByUsernameAsync(string username)
    {
        await using var conn = await dbFactory.CreateOpenConnectionAsync();

        // 第一次查询：用户基础信息
        const string userSql = @"
            SELECT id, username, password_hash, real_name, department_id, department_name, status, created_at, updated_at
            FROM sys_user
            WHERE username = @Username AND status = 1";

        var user = await conn.QueryFirstOrDefaultAsync<SysUser>(userSql, new { Username = username });

        if (user == null)
        {
            logger.LogWarning("用户不存在或已禁用: {Username}", username);
            return null;
        }

        // 第二次查询：该用户绑定的角色
        const string roleSql = @"
            SELECT r.role_code
            FROM sys_user_role ur
            INNER JOIN sys_role r ON r.id = ur.role_id
            WHERE ur.user_id = @UserId";

        var roles = await conn.QueryAsync<string>(roleSql, new { UserId = user.Id });
        user.Roles = roles.ToList();

        logger.LogInformation("用户 [{Username}] 查询成功，角色: [{Roles}]",
            username, string.Join(", ", user.Roles));

        return user;
    }

    /// <summary>根据 ID 查找用户</summary>
    public async Task<SysUser?> FindByIdAsync(int id)
    {
        await using var conn = await dbFactory.CreateOpenConnectionAsync();
        const string sql = "SELECT * FROM sys_user WHERE id = @Id AND status = 1";
        return await conn.QueryFirstOrDefaultAsync<SysUser>(sql, new { Id = id });
    }

    public async Task<bool> UpdateDepartmentAsync(int userId, int? deptId, string? deptName)
    {
        const string sql = "UPDATE sys_user SET department_id = @DeptId, department_name = @DeptName WHERE id = @UserId";
        await using var conn = await dbFactory.CreateOpenConnectionAsync();
        return await conn.ExecuteAsync(sql, new { DeptId = deptId, DeptName = deptName, UserId = userId }) > 0;
    }

    public async Task<IEnumerable<SysUser>> GetUsersByDepartmentAsync(int? deptId)
    {
        var sql = deptId.HasValue && deptId > 0
            ? "SELECT id, username, real_name, department_id, department_name FROM sys_user WHERE department_id = @DeptId AND status = 1"
            : "SELECT id, username, real_name, department_id, department_name FROM sys_user WHERE status = 1";
        
        await using var conn = await dbFactory.CreateOpenConnectionAsync();
        return await conn.QueryAsync<SysUser>(sql, new { DeptId = deptId });
    }
}

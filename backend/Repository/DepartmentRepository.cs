using Dapper;
using Letiao.ATS.Api.Domain.Entities;
using Letiao.ATS.Api.Infrastructure;

namespace Letiao.ATS.Api.Repository;

public class DepartmentRepository(DbConnectionFactory dbFactory)
{
    public async Task<int> CreateAsync(SysDepartment dept)
    {
        const string sql = @"
            INSERT INTO sys_department (name, parent_id, order_num, created_at, updated_at)
            VALUES (@Name, @ParentId, @OrderNum, NOW(), NOW());
            SELECT LAST_INSERT_ID();";

        await using var conn = await dbFactory.CreateOpenConnectionAsync();
        return await conn.ExecuteScalarAsync<int>(sql, dept);
    }

    public async Task<bool> UpdateAsync(int id, SysDepartment dept)
    {
        const string sql = @"
            UPDATE sys_department 
            SET name = @Name, parent_id = @ParentId, order_num = @OrderNum 
            WHERE id = @Id;";

        await using var conn = await dbFactory.CreateOpenConnectionAsync();
        return await conn.ExecuteAsync(sql, new { dept.Name, dept.ParentId, dept.OrderNum, Id = id }) > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM sys_department WHERE id = @Id;";
        await using var conn = await dbFactory.CreateOpenConnectionAsync();
        return await conn.ExecuteAsync(sql, new { Id = id }) > 0;
    }

    public async Task<IEnumerable<SysDepartment>> GetAllAsync()
    {
        const string sql = "SELECT * FROM sys_department ORDER BY order_num ASC, id ASC;";
        await using var conn = await dbFactory.CreateOpenConnectionAsync();
        return await conn.QueryAsync<SysDepartment>(sql);
    }
}

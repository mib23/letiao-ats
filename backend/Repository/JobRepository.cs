using Dapper;
using Letiao.ATS.Api.Application;
using Letiao.ATS.Api.Common;
using Letiao.ATS.Api.Domain;
using Letiao.ATS.Api.Infrastructure;

namespace Letiao.ATS.Api.Repository;

public class JobRepository(DbConnectionFactory dbFactory)
{
    public async Task<int> CreateAsync(JobPosting job)
    {
        const string sql = @"
            INSERT INTO ats_job_posting (title, department_name, city, headcount_target, min_salary, max_salary, description, status, hr_owner_id, created_at, updated_at)
            VALUES (@Title, @DepartmentName, @City, @HeadcountTarget, @MinSalary, @MaxSalary, @Description, @Status, @HrOwnerId, NOW(), NOW());
            SELECT LAST_INSERT_ID();";

        await using var conn = await dbFactory.CreateOpenConnectionAsync();
        return await conn.ExecuteScalarAsync<int>(sql, job);
    }

    public async Task<bool> UpdateAsync(JobPosting job)
    {
        const string sql = @"
            UPDATE ats_job_posting 
            SET title = @Title, department_name = @DepartmentName, city = @City, 
                headcount_target = @HeadcountTarget, min_salary = @MinSalary, max_salary = @MaxSalary, 
                description = @Description, status = @Status, hr_owner_id = @HrOwnerId
            WHERE id = @Id;";

        await using var conn = await dbFactory.CreateOpenConnectionAsync();
        return await conn.ExecuteAsync(sql, job) > 0;
    }

    public async Task<JobResponse?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT j.*, u.real_name AS HrOwnerName
            FROM ats_job_posting j
            LEFT JOIN sys_user u ON j.hr_owner_id = u.id
            WHERE j.id = @Id;";

        await using var conn = await dbFactory.CreateOpenConnectionAsync();
        return await conn.QueryFirstOrDefaultAsync<JobResponse>(sql, new { Id = id });
    }

    public async Task<PagedResult<JobResponse>> GetPagedListAsync(JobQueryRequest query)
    {
        var sqlWhere = "WHERE 1=1";
        if (!string.IsNullOrEmpty(query.Keyword)) sqlWhere += " AND j.title LIKE @Keyword";
        if (!string.IsNullOrEmpty(query.Status)) sqlWhere += " AND j.status = @Status";

        var sqlCount = $"SELECT COUNT(1) FROM ats_job_posting j {sqlWhere}";
        var sqlList = $@"
            SELECT j.*, u.real_name AS HrOwnerName
            FROM ats_job_posting j
            LEFT JOIN sys_user u ON j.hr_owner_id = u.id
            {sqlWhere}
            ORDER BY j.id DESC
            LIMIT @Limit OFFSET @Offset;";

        await using var conn = await dbFactory.CreateOpenConnectionAsync();
        var limit = query.PageSize;
        var offset = (query.Page - 1) * query.PageSize;
        var parameters = new { 
            Keyword = $"%{query.Keyword}%", 
            Status = query.Status, 
            Limit = limit, 
            Offset = offset 
        };

        var total = await conn.ExecuteScalarAsync<int>(sqlCount, parameters);
        var list = await conn.QueryAsync<JobResponse>(sqlList, parameters);

        return new PagedResult<JobResponse> { Total = total, List = list };
    }
}

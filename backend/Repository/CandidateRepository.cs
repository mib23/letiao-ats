using Dapper;
using Letiao.ATS.Api.Application;
using Letiao.ATS.Api.Domain;
using Letiao.ATS.Api.Infrastructure;
using Letiao.ATS.Api.Common;

namespace Letiao.ATS.Api.Repository;

public class CandidateRepository(DbConnectionFactory dbFactory)
{
    public async Task<int> CreateAsync(Candidate candidate)
    {
        const string sql = @"
            INSERT INTO ats_candidate (name, phone, email, gender, highest_degree, work_years, resume_file_url, ai_parsed_data, owner_id, last_follow_time, created_at, updated_at)
            VALUES (@Name, @Phone, @Email, @Gender, @HighestDegree, @WorkYears, @ResumeFileUrl, @AiParsedData, @OwnerId, NOW(), NOW(), NOW());
            SELECT LAST_INSERT_ID();";

        await using var conn = await dbFactory.CreateOpenConnectionAsync();
        return await conn.ExecuteScalarAsync<int>(sql, candidate);
    }

    public async Task<Candidate?> GetByPhoneAsync(string phone)
    {
        const string sql = "SELECT * FROM ats_candidate WHERE phone = @Phone LIMIT 1;";
        await using var conn = await dbFactory.CreateOpenConnectionAsync();
        return await conn.QueryFirstOrDefaultAsync<Candidate>(sql, new { Phone = phone });
    }

    public async Task<PagedResult<CandidateResponse>> GetPagedListAsync(CandidateQueryRequest query)
    {
        var sqlWhere = "WHERE 1=1";
        if (!string.IsNullOrEmpty(query.Keyword)) sqlWhere += " AND (c.name LIKE @Keyword OR c.phone LIKE @Keyword)";

        var sqlCount = $"SELECT COUNT(1) FROM ats_candidate c {sqlWhere}";
        var sqlList = $@"
            SELECT c.*, u.real_name AS OwnerName
            FROM ats_candidate c
            LEFT JOIN sys_user u ON c.owner_id = u.id
            {sqlWhere}
            ORDER BY c.id DESC
            LIMIT @Limit OFFSET @Offset;";

        await using var conn = await dbFactory.CreateOpenConnectionAsync();
        var limit = query.PageSize;
        var offset = (query.Page - 1) * query.PageSize;
        var parameters = new { 
            Keyword = $"%{query.Keyword}%", 
            Limit = limit, 
            Offset = offset 
        };

        var total = await conn.ExecuteScalarAsync<int>(sqlCount, parameters);
        var list = await conn.QueryAsync<CandidateResponse>(sqlList, parameters);

        return new PagedResult<CandidateResponse> { Total = total, List = list };
    }
}

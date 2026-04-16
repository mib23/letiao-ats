using Dapper;
using Letiao.ATS.Api.Domain.Entities;
using Letiao.ATS.Api.Infrastructure;

namespace Letiao.ATS.Api.Repository;

public class PipelineRepository(DbConnectionFactory dbFactory)
{
    public async Task<int> CreateAsync(int candidateId, int jobId, string stage)
    {
        const string sql = @"
            INSERT INTO ats_application_pipeline (candidate_id, job_id, current_stage, status_changed_at, created_at, updated_at)
            VALUES (@CandidateId, @JobId, @Stage, NOW(), NOW(), NOW());
            SELECT LAST_INSERT_ID();";
        await using var conn = await dbFactory.CreateOpenConnectionAsync();
        return await conn.ExecuteScalarAsync<int>(sql, new { CandidateId = candidateId, JobId = jobId, Stage = stage });
    }

    public async Task<bool> MoveStageAsync(int id, string targetStage)
    {
        const string sql = @"
            UPDATE ats_application_pipeline 
            SET current_stage = @Stage, status_changed_at = NOW() 
            WHERE id = @Id;";
        await using var conn = await dbFactory.CreateOpenConnectionAsync();
        return await conn.ExecuteAsync(sql, new { Id = id, Stage = targetStage }) > 0;
    }

    public async Task<IEnumerable<ApplicationPipeline>> GetByJobIdAsync(int jobId)
    {
        const string sql = @"
            SELECT p.*, c.name AS CandidateName, c.phone AS CandidatePhone, c.highest_degree AS CandidateHighestDegree, c.work_years AS CandidateWorkYears
            FROM ats_application_pipeline p
            INNER JOIN ats_candidate c ON p.candidate_id = c.id
            WHERE p.job_id = @JobId AND p.is_eliminated = 0
            ORDER BY p.status_changed_at DESC;";
        await using var conn = await dbFactory.CreateOpenConnectionAsync();
        return await conn.QueryAsync<ApplicationPipeline>(sql, new { JobId = jobId });
    }
}

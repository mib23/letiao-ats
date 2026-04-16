using Letiao.ATS.Api.Domain.Entities;
using Letiao.ATS.Api.Repository;

namespace Letiao.ATS.Api.Application;

public class PipelineDto
{
    public int CandidateId { get; set; }
    public int JobId { get; set; }
}

public class PipelineMoveRequest
{
    public string TargetStage { get; set; } = string.Empty;
}

public class PipelineService(PipelineRepository repo)
{
    public Task<int> AddCandidateToJobAsync(PipelineDto dto)
    {
        return repo.CreateAsync(dto.CandidateId, dto.JobId, "SCREENING");
    }

    public Task<bool> MoveStageAsync(int id, string targetStage)
    {
        return repo.MoveStageAsync(id, targetStage);
    }

    public Task<IEnumerable<ApplicationPipeline>> GetBoardDataAsync(int jobId)
    {
        return repo.GetByJobIdAsync(jobId);
    }
}

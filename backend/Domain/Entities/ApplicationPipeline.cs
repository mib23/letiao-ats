namespace Letiao.ATS.Api.Domain.Entities;

public class ApplicationPipeline
{
    public int Id { get; set; }
    public int CandidateId { get; set; }
    public int JobId { get; set; }
    public string CurrentStage { get; set; } = string.Empty;
    public bool IsEliminated { get; set; }
    public string? EliminationReason { get; set; }
    public DateTime StatusChangedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string CandidateName { get; set; } = string.Empty;
    public string CandidatePhone { get; set; } = string.Empty;
    public string CandidateHighestDegree { get; set; } = string.Empty;
    public int CandidateWorkYears { get; set; }
}

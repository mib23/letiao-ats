namespace Letiao.ATS.Api.Domain;

public class Candidate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int Gender { get; set; }
    public string? HighestDegree { get; set; }
    public int WorkYears { get; set; }
    public string? ResumeFileUrl { get; set; }
    public string? AiParsedData { get; set; }
    public int? OwnerId { get; set; }
    public DateTime LastFollowTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

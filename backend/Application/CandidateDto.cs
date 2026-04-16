using Letiao.ATS.Api.Domain;

namespace Letiao.ATS.Api.Application;

public class CandidateSaveRequest
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int Gender { get; set; }
    public string? HighestDegree { get; set; }
    public int WorkYears { get; set; }
    public string? ResumeFileUrl { get; set; }
    public string? AiParsedData { get; set; }
}

public class CandidateQueryRequest
{
    public string? Keyword { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class CandidateResponse : Candidate
{
    public string? OwnerName { get; set; }
}

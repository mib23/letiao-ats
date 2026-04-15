using Letiao.ATS.Api.Domain;

namespace Letiao.ATS.Api.Application;

public class JobSaveRequest
{
    public string Title { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int HeadcountTarget { get; set; } = 1;
    public int MinSalary { get; set; }
    public int MaxSalary { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "PUBLISHED";
    public int HrOwnerId { get; set; }
}

public class JobQueryRequest
{
    public string? Keyword { get; set; }
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class JobResponse : JobPosting
{
    public string HrOwnerName { get; set; } = string.Empty;
}

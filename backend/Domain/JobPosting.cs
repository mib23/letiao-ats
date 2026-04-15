namespace Letiao.ATS.Api.Domain;

public class JobPosting
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int HeadcountTarget { get; set; } = 1;
    public int MinSalary { get; set; }
    public int MaxSalary { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "PUBLISHED";
    public int HrOwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

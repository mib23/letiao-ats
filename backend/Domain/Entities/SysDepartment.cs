namespace Letiao.ATS.Api.Domain.Entities;

public class SysDepartment
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ParentId { get; set; } = 0;
    public int OrderNum { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

namespace Letiao.ATS.Api.Domain.Entities;

/// <summary>系统账号实体</summary>
public class SysUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int Status { get; set; } = 1;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    /// <summary>关联角色列表（连表查询时填充）</summary>
    public List<string> Roles { get; set; } = new();
}

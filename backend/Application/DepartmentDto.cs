namespace Letiao.ATS.Api.Application;

public class DepartmentSaveRequest
{
    public string Name { get; set; } = string.Empty;
    public int ParentId { get; set; } = 0;
    public int OrderNum { get; set; } = 0;
}

public class DepartmentTreeNode
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ParentId { get; set; }
    public int OrderNum { get; set; }
    public List<DepartmentTreeNode> Children { get; set; } = new();
}

public class UserDepartmentMappingRequest
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
}

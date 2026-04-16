using Letiao.ATS.Api.Domain.Entities;
using Letiao.ATS.Api.Repository;

namespace Letiao.ATS.Api.Application;

public class DepartmentService(DepartmentRepository deptRepo)
{
    public async Task<int> CreateDepartmentAsync(DepartmentSaveRequest request)
    {
        var dept = new SysDepartment
        {
            Name = request.Name,
            ParentId = request.ParentId,
            OrderNum = request.OrderNum
        };
        return await deptRepo.CreateAsync(dept);
    }

    public Task<bool> UpdateDepartmentAsync(int id, DepartmentSaveRequest request)
    {
        var dept = new SysDepartment
        {
            Name = request.Name,
            ParentId = request.ParentId,
            OrderNum = request.OrderNum
        };
        return deptRepo.UpdateAsync(id, dept);
    }

    public Task<bool> DeleteDepartmentAsync(int id) => deptRepo.DeleteAsync(id);

    public async Task<List<DepartmentTreeNode>> GetDepartmentTreeAsync()
    {
        var allDepts = await deptRepo.GetAllAsync();
        var nodes = allDepts.Select(d => new DepartmentTreeNode
        {
            Id = d.Id,
            Name = d.Name,
            ParentId = d.ParentId,
            OrderNum = d.OrderNum
        }).ToList();

        // Build Tree
        var dict = nodes.ToDictionary(n => n.Id);
        var roots = new List<DepartmentTreeNode>();

        foreach (var node in nodes)
        {
            if (node.ParentId == 0 || !dict.ContainsKey(node.ParentId))
            {
                roots.Add(node);
            }
            else
            {
                dict[node.ParentId].Children.Add(node);
            }
        }

        return roots;
    }
}

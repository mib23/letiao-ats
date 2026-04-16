using Letiao.ATS.Api.Application;
using Letiao.ATS.Api.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Letiao.ATS.Api.Controllers;

[ApiController]
[Route("api/departments")]
[Authorize]
public class DepartmentController(DepartmentService deptService) : ControllerBase
{
    [HttpGet("tree")]
    public async Task<IActionResult> GetTree()
    {
        var tree = await deptService.GetDepartmentTreeAsync();
        return Ok(ApiResponse.Ok(tree));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DepartmentSaveRequest req)
    {
        var id = await deptService.CreateDepartmentAsync(req);
        return Ok(ApiResponse.Ok(new { id }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] DepartmentSaveRequest req)
    {
        await deptService.UpdateDepartmentAsync(id, req);
        return Ok(ApiResponse.Ok<object?>(null));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await deptService.DeleteDepartmentAsync(id);
        return Ok(ApiResponse.Ok<object?>(null));
    }
}

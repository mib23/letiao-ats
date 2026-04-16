using Letiao.ATS.Api.Application;
using Letiao.ATS.Api.Common;
using Letiao.ATS.Api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Letiao.ATS.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController(UserRepository userRepo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int? departmentId)
    {
        var users = await userRepo.GetUsersByDepartmentAsync(departmentId);
        return Ok(ApiResponse.Ok(users));
    }

    [HttpPut("{id}/department")]
    public async Task<IActionResult> UpdateDepartment(int id, [FromBody] UserDepartmentMappingRequest req)
    {
        var success = await userRepo.UpdateDepartmentAsync(id, req.DepartmentId, req.DepartmentName);
        if (!success) return BadRequest(ApiResponse.Fail(400, "更新失败"));
        return Ok(ApiResponse.Ok<object?>(null));
    }
}

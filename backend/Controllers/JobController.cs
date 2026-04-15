using Letiao.ATS.Api.Application;
using Letiao.ATS.Api.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Letiao.ATS.Api.Controllers;

/// <summary>
/// 职位管理接口
/// </summary>
[ApiController]
[Route("api/jobs")]
[Authorize]
public class JobController(JobService jobService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] JobQueryRequest query)
    {
        var result = await jobService.GetFilteredJobsAsync(query);
        return Ok(ApiResponse.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await jobService.GetJobAsync(id);
        if (result == null) return NotFound(ApiResponse.Fail(404, "职位不存在"));
        return Ok(ApiResponse.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] JobSaveRequest request)
    {
        // 如果不指定 HR，则可默认为当前操作人
        if (request.HrOwnerId == 0)
        {
             var uid = User.FindFirst("uid")?.Value;
             if (int.TryParse(uid, out int parsedUid))
                request.HrOwnerId = parsedUid;
        }

        var id = await jobService.CreateJobAsync(request);
        return Ok(ApiResponse.Ok(new { id }, "职位创建成功"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] JobSaveRequest request)
    {
        var success = await jobService.UpdateJobAsync(id, request);
        if (!success) return NotFound(ApiResponse.Fail(404, "职位不存在或更新失败"));
        return Ok(ApiResponse.Ok<object?>(null, "职位更新成功"));
    }
}

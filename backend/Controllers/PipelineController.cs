using Letiao.ATS.Api.Application;
using Letiao.ATS.Api.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Letiao.ATS.Api.Controllers;

[ApiController]
[Route("api/pipeline")]
[Authorize]
public class PipelineController(PipelineService svc) : ControllerBase
{
    [HttpGet("jobs/{jobId}")]
    public async Task<IActionResult> GetBoardData(int jobId)
    {
        var data = await svc.GetBoardDataAsync(jobId);
        return Ok(ApiResponse.Ok(data));
    }

    [HttpPost]
    public async Task<IActionResult> AddCandidate([FromBody] PipelineDto req)
    {
        try
        {
            var id = await svc.AddCandidateToJobAsync(req);
            return Ok(ApiResponse.Ok(new { id }));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.Fail(400, "添加候选人失败，可能是重复投递: " + ex.Message));
        }
    }

    [HttpPut("{id}/move")]
    public async Task<IActionResult> MoveStage(int id, [FromBody] PipelineMoveRequest req)
    {
        await svc.MoveStageAsync(id, req.TargetStage);
        return Ok(ApiResponse.Ok<object?>(null));
    }
}

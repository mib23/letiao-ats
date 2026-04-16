using Letiao.ATS.Api.Application;
using Letiao.ATS.Api.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Letiao.ATS.Api.Controllers;

/// <summary>
/// 候选人管理接口
/// </summary>
[ApiController]
[Route("api/candidates")]
[Authorize]
public class CandidateController(CandidateService candidateService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] CandidateQueryRequest query)
    {
        var result = await candidateService.GetFilteredCandidatesAsync(query);
        return Ok(ApiResponse.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CandidateSaveRequest request)
    {
        int? uid = null;
        var uidStr = User.FindFirst("uid")?.Value;
        if (int.TryParse(uidStr, out int parsedUid))
            uid = parsedUid;

        var id = await candidateService.CreateCandidateAsync(request, uid);
        return Ok(ApiResponse.Ok(new { id }, "候选人创建成功"));
    }
}

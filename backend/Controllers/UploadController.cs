using Letiao.ATS.Api.Common;
using Letiao.ATS.Api.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Letiao.ATS.Api.Controllers;

/// <summary>
/// 文件上传接口（OSS 统一入口）
/// </summary>
[ApiController]
[Route("api/upload")]
[Authorize]
public class UploadController(IOssService ossService) : ControllerBase
{
    /// <summary>
    /// 上传简历/附件（支持 PDF/Word/图片，最大 20MB）
    /// </summary>
    [HttpPost("resume")]
    public async Task<IActionResult> UploadResume(IFormFile file)
    {
        try
        {
            var url = await ossService.UploadAsync(file, "resumes");
            return Ok(ApiResponse.Ok(new { url }, "上传成功"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse.Fail(400, ex.Message));
        }
    }

    /// <summary>
    /// 上传头像/图片
    /// </summary>
    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        try
        {
            var url = await ossService.UploadAsync(file, "avatars");
            return Ok(ApiResponse.Ok(new { url }, "上传成功"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse.Fail(400, ex.Message));
        }
    }
}

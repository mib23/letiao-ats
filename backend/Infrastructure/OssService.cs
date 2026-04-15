namespace Letiao.ATS.Api.Infrastructure;

/// <summary>
/// OSS 文件存储服务接口（抽象层，便于后续切换至阿里云OSS/MinIO）
/// </summary>
public interface IOssService
{
    /// <summary>上传文件，返回可访问的 URL</summary>
    Task<string> UploadAsync(IFormFile file, string folder = "uploads");
}

/// <summary>
/// 本地文件系统实现（开发/测试阶段使用，生产可替换为云OSS实现）
/// </summary>
public class LocalOssService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, ILogger<LocalOssService> logger) : IOssService
{
    private const long MaxFileSizeBytes = 20 * 1024 * 1024; // 20MB

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".webp"
    };

    public async Task<string> UploadAsync(IFormFile file, string folder = "uploads")
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("上传文件不能为空");

        if (file.Length > MaxFileSizeBytes)
            throw new ArgumentException($"文件大小超过限制（最大 {MaxFileSizeBytes / 1024 / 1024}MB）");

        var ext = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(ext))
            throw new ArgumentException($"不支持的文件类型: {ext}");

        // 生成唯一文件名，避免覆盖
        var uniqueFileName = $"{Guid.NewGuid():N}{ext}";
        var uploadDir = Path.Combine(env.WebRootPath ?? env.ContentRootPath, "static", folder);
        Directory.CreateDirectory(uploadDir);

        var filePath = Path.Combine(uploadDir, uniqueFileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        logger.LogInformation("文件已上传: {FileName} -> {FilePath}", file.FileName, filePath);

        // 拼接可访问的 URL
        var request = httpContextAccessor.HttpContext?.Request;
        var baseUrl = request != null
            ? $"{request.Scheme}://{request.Host}"
            : "http://localhost:5000";

        return $"{baseUrl}/static/{folder}/{uniqueFileName}";
    }
}

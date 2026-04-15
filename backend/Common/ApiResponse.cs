namespace Letiao.ATS.Api.Common;

/// <summary>
/// 全局统一响应格式 { code, msg, data }
/// </summary>
public class ApiResponse<T>
{
    public int Code { get; set; }
    public string Msg { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string msg = "success") =>
        new() { Code = 200, Msg = msg, Data = data };

    public static ApiResponse<object> Fail(int code, string msg) =>
        new ApiResponse<object> { Code = code, Msg = msg, Data = null };
}

public static class ApiResponse
{
    public static ApiResponse<T> Ok<T>(T data, string msg = "success") =>
        new() { Code = 200, Msg = msg, Data = data };

    public static ApiResponse<object> Fail(int code = 500, string msg = "服务器内部错误") =>
        new ApiResponse<object> { Code = code, Msg = msg, Data = null };
}

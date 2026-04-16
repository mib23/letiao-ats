using Letiao.ATS.Api.Common;

namespace Letiao.ATS.Api.Common;

public class BusinessException : Exception
{
    public int Code { get; }

    public BusinessException(int code, string message) : base(message)
    {
        Code = code;
    }

    public BusinessException(string message) : base(message)
    {
        Code = 400; // 默认业务异常状态码为 400
    }
}

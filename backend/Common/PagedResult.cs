namespace Letiao.ATS.Api.Common;

public class PagedResult<T>
{
    public int Total { get; set; }
    public IEnumerable<T> List { get; set; } = new List<T>();
}

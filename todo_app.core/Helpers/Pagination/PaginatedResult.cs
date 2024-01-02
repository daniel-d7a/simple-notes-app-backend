namespace todo_app.core.Helpers.Pagination;
public class PaginatedResult <T>
{
    public int Page { get; private set; }
    public int PageSize { get; private set; }
    public int LastPage { get; private set; }
    public int Total { get; private set; }
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < LastPage;
    public IEnumerable<T> Data { get; set; }

    public PaginatedResult(IEnumerable<T> items, int page, int pageSize, int total)
    {
        Page = page;
        PageSize = pageSize;
        LastPage = (int)Math.Ceiling(total / (double)pageSize);
        Total = total;
        Data = items;
    }

    public static PaginatedResult<T> ToPaginatedList(IEnumerable<T> source, int page, int pageSize)
    {
        int total = source.Count();
        int skip = (page - 1) * pageSize;
        var items = source.Skip(skip).Take(pageSize);

        return new PaginatedResult<T>(items, page, pageSize, total);
    }

}

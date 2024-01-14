namespace todo_app.core.Helpers.QueryParams;

public class BaseQueryParams
{
    public static readonly int MAX_PAGES = 50;
    public int Page { get; set; } = 1;

    private int pageSize = 10;
    public int PageSize
    {
        get => pageSize;
        set => pageSize = value < MAX_PAGES ? value : MAX_PAGES;
    }

    public string Type { get; set; } = "";
}

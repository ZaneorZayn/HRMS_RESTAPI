namespace hrms_api.Helper;

public class PagedResult<T>
{
    public IEnumerable<T> Data { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItem { get; set; }
    public int TotalPages { get; set; }
    
}

public class QueryParameters
{
    private const int MaxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    
    private int _pageSize = 10;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    public string? SortBy { get; set; } = "Id";
    public string? SortOrder { get; set; } = "asc";
    public string? Search { get; set; } 
}
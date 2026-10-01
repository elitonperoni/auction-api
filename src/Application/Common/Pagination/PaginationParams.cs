namespace Application.Common.Pagination;

public record PaginationParams
{
    private readonly int _pageIndex = 1;
    public int PageIndex
    {
        get => _pageIndex;
        init => _pageIndex = value < 1 ? 1 : value;
    }
    private readonly int _pageSize = 20; 
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > 50 ? 50 : value;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Pagination;

public class Pagination<T> : List<T>
{
    public int PageIndex { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }

    public Pagination(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        PageIndex = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        AddRange(items);
    }   
    public PaginationMetadata GetMetadata()
    {
        return new PaginationMetadata
        {
            TotalCount = TotalCount,
            PageSize = PageSize,
            PageIndex = PageIndex,
            TotalPages = TotalPages,
        };
    }
}
public class PaginationMetadata
{
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }
}


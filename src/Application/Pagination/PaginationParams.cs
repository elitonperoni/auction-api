using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Pagination;

public record PaginationParams
{
    public int PageIndex { get; set; }
    private readonly int _pageSize = 20; 
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > 50 ? 50 : value;
    }
}

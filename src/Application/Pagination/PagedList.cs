using Microsoft.EntityFrameworkCore;

namespace Application.Pagination;

public static class PagedList<T>
{
    public static async Task<Pagination<T>> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
    {
        int count = await source.CountAsync();
        List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new Pagination<T>(items, count, pageNumber, pageSize);
    }
}

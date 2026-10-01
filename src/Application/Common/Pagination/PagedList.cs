using Microsoft.EntityFrameworkCore;

namespace Application.Common.Pagination;

public static class PagedList<T>
{
    public static async Task<Pagination<T>> ToPagedList(
        IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = Math.Max(pageNumber, 1);

        int count = await source.CountAsync(cancellationToken);
        List<T> items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new Pagination<T>(items, count, pageNumber, pageSize);
    }
}

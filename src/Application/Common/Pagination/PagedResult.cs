namespace Application.Common.Pagination;

public record PagedResult<T>(
    List<T> Items,
    PaginationMetadata metaData);

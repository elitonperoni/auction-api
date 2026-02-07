namespace Application.Pagination;

public record PagedResult<T>(
    List<T> Items,
    PaginationMetadata metaData);

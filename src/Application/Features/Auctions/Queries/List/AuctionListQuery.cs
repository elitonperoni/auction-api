using Application.Common.Abstractions.Messaging;
using Application.Common.Pagination;

namespace Application.Features.Auctions.Queries.List;

public sealed record AuctionListQuery(string? SearchTerm) : PaginationParams, IQuery<PagedResult<AuctionListResponse>>;

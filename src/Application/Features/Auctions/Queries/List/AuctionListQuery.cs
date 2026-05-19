using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Abstractions.Messaging;
using Application.Common.Pagination;

namespace Application.Features.Auctions.Queries.List;

public sealed record AuctionListQuery(string? SearchTerm) : PaginationParams, IQuery<PagedResult<AuctionListResponse>>;

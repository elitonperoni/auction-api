using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Pagination;

namespace Application.AuctionUseCases.List;

public sealed record AuctionListQuery(string? SearchTerm) : PaginationParams, IQuery<PagedResult<AuctionListResponse>>;

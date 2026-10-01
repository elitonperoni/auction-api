using Application.Common.Abstractions.Messaging;

namespace Application.Features.Auctions.Queries.ListByUserId;

public sealed record AuctionListByUserIdQuery() : IQuery<List<AuctionListByUserIdResponse>>;



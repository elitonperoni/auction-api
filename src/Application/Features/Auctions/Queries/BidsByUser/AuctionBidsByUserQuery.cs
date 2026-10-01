using Application.Common.Abstractions.Messaging;

namespace Application.Features.Auctions.Queries.BidsByUser;

public sealed record AuctionBidsByUserQuery() : IQuery<List<AuctionBidsByUserResponse>>;

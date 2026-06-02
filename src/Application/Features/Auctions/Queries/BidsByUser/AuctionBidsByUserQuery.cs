using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Abstractions.Messaging;

namespace Application.Features.Auctions.Queries.BidsByUser;

public sealed record AuctionBidsByUserQuery() : IQuery<List<AuctionBidsByUserResponse>>;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;

namespace Application.AuctionUseCases.BidsByUser;

public sealed record AuctionBidsByUserQuery() : IQuery<List<AuctionBidsByUserResponse>>;

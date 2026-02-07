using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.AuctionUseCases.GetDetail;

namespace Application.AuctionUseCases.Update;

public sealed record GetRegisterDetailQuery(Guid Id) : IQuery<GetRegisterDetailResponse>;

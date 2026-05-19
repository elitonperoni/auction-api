using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Abstractions.Messaging;

namespace Application.Features.Auctions.Queries.GetDetail;

public sealed record GetDetailProductQuery(Guid Id) : IQuery<GetDetailProductResponse>;

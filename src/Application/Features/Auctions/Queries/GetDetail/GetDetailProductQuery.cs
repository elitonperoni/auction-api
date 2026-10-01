using Application.Common.Abstractions.Messaging;

namespace Application.Features.Auctions.Queries.GetDetail;

public sealed record GetDetailProductQuery(Guid Id) : IQuery<GetDetailProductResponse>;

using Application.Common.Abstractions.Messaging;

namespace Application.Features.Auctions.Queries.GetRegisterDetail;

public sealed record GetRegisterDetailQuery(Guid Id) : IQuery<GetRegisterDetailResponse>;

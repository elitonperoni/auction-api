using Application.Common.Abstractions.Messaging;

namespace Application.Features.Auctions.Commands.Update;

public sealed record GetRegisterDetailQuery(Guid Id) : IQuery<GetRegisterDetailResponse>;

using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Queries.GetById;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;

using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Queries.GetByEmail;

public sealed record GetUserByEmailQuery(string Email) : IQuery<UserResponse>;

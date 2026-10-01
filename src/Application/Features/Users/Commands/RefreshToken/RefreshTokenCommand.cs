using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string Token, string RefreshToken) : ICommand<RefreshTokenResponse>;

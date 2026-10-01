using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Command.RefreshToken;

public sealed record RefreshTokenCommand(string Token, string RefreshToken) : ICommand<RefreshTokenResponse>;

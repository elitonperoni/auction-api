using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Commands.Login;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<LoginResponse>;

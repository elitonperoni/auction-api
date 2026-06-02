using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Command.Login;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<LoginResponse>;

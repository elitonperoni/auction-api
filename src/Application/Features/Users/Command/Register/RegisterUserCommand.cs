using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Command.Register;

public sealed record RegisterUserCommand(string Email, string FirstName, string LastName, string Password)
    : ICommand<RegisterUserResponse>;

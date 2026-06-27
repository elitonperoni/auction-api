using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Command.Register;

public sealed record RegisterUserCommand(string Email, string FullName, string UserName,
   string Phone, string Location, string Country, string State, string City,
   string Language, string Timezone, string Password)
    : ICommand<RegisterUserResponse>;

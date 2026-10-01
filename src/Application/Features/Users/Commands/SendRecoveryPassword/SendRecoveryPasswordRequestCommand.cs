using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Commands.SendRecoveryPassword;

public sealed record SendRecoveryPasswordRequestCommand(string email) : ICommand<bool>;

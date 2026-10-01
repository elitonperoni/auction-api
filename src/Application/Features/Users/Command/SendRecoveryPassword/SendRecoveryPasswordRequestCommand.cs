using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Command.SendRecoveryPassword;

public sealed record SendRecoveryPasswordRequestCommand(string email) : ICommand<bool>;

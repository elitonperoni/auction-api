using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Commands.RecoveryPassword;

public sealed record RecoveryPasswordCommand(string token, string password) : ICommand<bool>;

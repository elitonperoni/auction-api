using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Commands.ResetPassword;

public sealed record ResetPasswordCommand(string ActualPassword, string NewPassword) : ICommand<Guid>;

using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Command.ResetPassword;

public sealed record ResetPasswordCommand(string ActualPassword, string NewPassword) : ICommand<Guid>;

using Application.Abstractions.Messaging;

namespace Application.Users.ResetPassword;

public sealed record ResetPasswordCommand(string ActualPassword, string NewPassword) : ICommand<Guid>;

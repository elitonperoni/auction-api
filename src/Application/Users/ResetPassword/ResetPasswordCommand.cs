using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;

namespace Application.Users.ResetPassword;

public sealed record ResetPasswordCommand(string token, string password) : ICommand<bool>;

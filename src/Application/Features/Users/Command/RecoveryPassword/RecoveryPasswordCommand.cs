using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Command.RecoveryPassword;

public sealed record RecoveryPasswordCommand(string token, string password) : ICommand<bool>;

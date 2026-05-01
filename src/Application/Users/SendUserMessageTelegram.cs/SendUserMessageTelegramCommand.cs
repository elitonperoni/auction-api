using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;

namespace Application.Users.SendUserMessageTelegram.cs;

public sealed record SendUserMessageTelegramCommand(Guid UserId, string Message) : ICommand<bool>;

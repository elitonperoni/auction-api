using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.Common.Abstractions.Messaging;

namespace Application.Features.Users.Command.RefreshToken;

public sealed record RefreshTokenCommand(string Token, string RefreshToken) : ICommand<RefreshTokenResponse>;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Abstractions.Messaging;
using Application.Common.DTOs;

namespace Application.Features.Users.Queries.GetNotifications;
public sealed record GetNotificationsQuery() : IQuery<List<NotificationItem>>;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Users.Notifications;
public sealed record GetNotificationsQuery() : IQuery<List<NotificationItem>>;


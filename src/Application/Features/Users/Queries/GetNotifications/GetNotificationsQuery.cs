using Application.Common.Abstractions.Messaging;
using Application.Common.DTOs;

namespace Application.Features.Users.Queries.GetNotifications;
public sealed record GetNotificationsQuery() : IQuery<List<NotificationItem>>;


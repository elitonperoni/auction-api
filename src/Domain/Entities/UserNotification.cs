using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities;

public sealed class UserNotification : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int NotificationTypeId { get; set; }
    public User? User { get; set; }
    public NotificationType? NotificationType { get; set; }
}

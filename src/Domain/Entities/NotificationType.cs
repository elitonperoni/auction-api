using SharedKernel;

namespace Domain.Entities;

public sealed class NotificationType : Entity
{
    public int Id { get; set; }
    public string Name { get; set; }
}

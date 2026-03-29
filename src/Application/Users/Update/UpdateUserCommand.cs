using Application.Abstractions.Messaging;

namespace Application.Users.Update;

public sealed record UpdateUserCommand : ICommand<Guid>
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string Phone { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public int Language { get; set; }
    public string TimeZone { get; set; }
} 

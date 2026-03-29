using System.ComponentModel.DataAnnotations;
using Domain.Entities;

namespace Application.Users.GetById;

public sealed record UserResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; }
    public string UserName { get; init; }
    public string CompleteName { get; init; }
    public string? Phone { get; set; }
    public string Country { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public int LanguageId { get; set; }
    public string TimeZone { get; set; }
    public DateTime MemberSince { get; set; }
    public List<int> UserNotifications { get; set; }
}

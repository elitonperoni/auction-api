using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SharedKernel;

namespace Domain.Entities;

public sealed class User : Entity
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    [StringLength(100)]
    public string UserName { get; set; }

    [Required]
    [StringLength(100)]
    public string CompleteName { get; set; }
    public string PasswordHash { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string? ResetPasswordCode { get; set; }
    public DateTime? ResetPasswordExpiry { get; set; }
    public string? Phone { get; set; }
    public string Country { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public int Language { get; set; }
    public string TimeZone { get; set; }
    public string? TelegramChatId { get; set; }

    [Column(TypeName = "timestamp with time zone")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "timestamp with time zone")]
    public DateTime LastUpdateDate { get; set; }
    public ICollection<UserNotification> UserNotifications { get; set; }
}

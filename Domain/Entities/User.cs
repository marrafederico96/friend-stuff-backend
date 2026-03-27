using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FriendStuff.Domain.Entities;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Username { get; set; }


    [Required]
    [MaxLength(100)]
    public required string NormalizedUsername { get; set; }

    [Required]
    [MaxLength(100)]
    public required string EmailAddress { get; set; }

    [Required]
    [MaxLength(100)]
    public required string NormalizedEmailAddress { get; set; }

    [Required]
    [MaxLength(600)]
    public required string PasswordHash { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


    // Navigation property
    public List<RefreshToken> RefreshTokens { get; set; } = [];
    public List<UserActivity> Activities { get; set; } = [];
    public List<Activity> OrganizedActivities { get; set; } = [];
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FriendStuff.Domain.Enums;

namespace FriendStuff.Domain.Entities;

public class UserActivity
{
    [Required]
    public required int UserId { get; set; }

    [Required]
    public int ActivityId { get; set; }

    [Required]
    public required UserRole Role { get; set; }

    [Required]
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;


    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [ForeignKey(nameof(ActivityId))]
    public Activity? Activity { get; set; }

}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace FriendStuff.Domain.Entities;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required int UserId { get; set; }

    [Required]
    public bool Valid { get; set; } = true;

    [Required]
    [MaxLength(600)]
    public required string TokenHash { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime ExpireAt { get; set; } = DateTime.UtcNow.AddDays(14);

    // Navigation property
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

}

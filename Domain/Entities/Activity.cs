using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FriendStuff.Domain.Enums;

namespace FriendStuff.Domain.Entities;

public class Activity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Guid PublicId { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(100)]
    public required string NormalizedName { get; set; }

    [Required]
    public required int AdminId { get; set; }

    [MaxLength(256)]
    public string? Description { get; set; }

    [Required]
    public ActivityType Type { get; set; }

    [Required]
    public required DateOnly StartDate { get; set; }

    [Required]
    public required DateOnly EndDate { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public List<UserActivity> Participants { get; set; } = [];

    [ForeignKey(nameof(AdminId))]
    public User? Admin { get; set; }

}

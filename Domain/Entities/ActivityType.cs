using System.ComponentModel.DataAnnotations;

namespace FriendStuff.Domain.Entities;

public class ActivityType
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Guid PublicId { get; set; } = Guid.NewGuid();

    [Required]
    public required string Name { get; set; }

    [Required]
    public required string NormalizedName { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public List<Activity> Activities { get; set; } = [];
    }

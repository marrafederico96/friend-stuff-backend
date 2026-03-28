using System.ComponentModel.DataAnnotations;
using FriendStuff.Domain.Enums;

namespace FriendStuff.Features.Activities.DTOs;

public record class CreateActivityRequest
{
    [Required(ErrorMessage = "Activity name is required")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(256)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Activity type is required")]
    public ActivityType Type { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    public DateOnly EndDate { get; set; }
}

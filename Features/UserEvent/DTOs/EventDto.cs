using System.ComponentModel.DataAnnotations;

namespace FriendStuffBackend.Features.Event.DTOs;

public record EventDto
{
    [Required(ErrorMessage = "Event name cannot be empty.")]
    public required string EventName { get; init; }
    
    [Required(ErrorMessage = "Start date cannot be empty.")]
    public DateOnly StartDate { get; set; }
    
    [Required(ErrorMessage = "End date cannot be empty.")]
    public DateOnly EndDate { get; set; }
    
    [Required(ErrorMessage = "Admin email required")]
    public required string AdminEmail { get; set; }
}
using System.ComponentModel.DataAnnotations;
using FriendStuffBackend.Domain.Entities;

namespace FriendStuffBackend.Features.UserEvent.DTOs;

public record EventDto
{
    [Required(ErrorMessage = "UserEvent name cannot be empty.")]
    public required string EventName { get; init; }

    public string NormalizedEventName { get; init; } = string.Empty;
    
    [Required(ErrorMessage = "Start date cannot be empty.")]
    public DateOnly StartDate { get; set; }
    
    [Required(ErrorMessage = "End date cannot be empty.")]
    public DateOnly EndDate { get; set; }
    
    [Required(ErrorMessage = "Admin email required")]
    public required string AdminEmail { get; set; }

    public List<EventUserDto?> Participants { get; set; } = [];
}
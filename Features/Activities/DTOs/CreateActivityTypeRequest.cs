using System.ComponentModel.DataAnnotations;

namespace FriendStuff.Features.Activities.DTOs;

public record CreateActivityTypeRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

}
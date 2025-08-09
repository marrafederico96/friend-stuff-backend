using System.ComponentModel.DataAnnotations;

namespace FriendStuffBackend.Features.UserEvent.DTOs;

public record EventMemberDto
{
    [Required(ErrorMessage = "Participant Username required")]
    public required string UserName { get; set; }
    
    [Required(ErrorMessage = "Event name required")]
    public required string NormalizedEventName { get; set; }
    
    [Required(ErrorMessage = "Admin username required")]
    public required string AdminUsername { get; set; }
}
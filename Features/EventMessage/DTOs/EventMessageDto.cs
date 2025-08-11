using System.ComponentModel.DataAnnotations;

namespace FriendStuffBackend.Features.EventMessage.DTOs;

public record EventMessageDto
{
    [Required(ErrorMessage = "Message content required")]
    public required string MessageContent { get; set; }
    
    [Required(ErrorMessage = "Sender required")]
    public required string SenderUsername { get; set; }
    
    [Required(ErrorMessage = "Event name required")]
    public required string NormalizedEventName { get; set; }
}
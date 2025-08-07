using System.ComponentModel.DataAnnotations;
using FriendStuffBackend.Features.Account.DTOs;

namespace FriendStuffBackend.Features.ExpenseEvent.DTOs;

public record ExpenseParticipantDto
{
    [Required(ErrorMessage = "Participants cannot be empty")]
    public required List<SearchUserDto> ListParticipants = [];
    
    
    
    public required string EventName { get; set; }
    
    
    
}
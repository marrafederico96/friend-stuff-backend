using System.ComponentModel.DataAnnotations;

namespace FriendStuffBackend.Features.ExpenseEvent.DTOs;

public record ExpenseBalanceDto
{
    [Required(ErrorMessage = "Username required")]
    public required string LoggedUsername { get; set; }
    
}
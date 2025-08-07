using System.ComponentModel.DataAnnotations;

namespace FriendStuffBackend.Features.ExpenseEvent.DTOs;

public record ExpenseEventDto
{
    [Required(ErrorMessage = "Expense name cannot be empty")]
    public required string ExpenseName { get; set; }
    
    [Required(ErrorMessage = "Amount cannot be empty")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public required decimal Amount { get; set; }
    
    public required string PayerUsername { get; set; }
    public required string EventName { get; set; }

    public List<ExpenseParticipantDto> ExpenseParticipant { get; set; } = [];
}
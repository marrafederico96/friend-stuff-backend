using System.ComponentModel.DataAnnotations;

namespace FriendStuffBackend.Features.ExpenseEvent.DTOs;

public record ExpenseEventDto
{
    [Required(ErrorMessage = "Expense name cannot be empty")]
    public required string ExpenseName { get; init; }
    
    [Required(ErrorMessage = "Amount cannot be empty")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public required decimal Amount { get; init; }
    
    public required string PayerUsername { get; init; }
    public required string EventName { get; init; }

    [Required(ErrorMessage = "Expense participant cannot be empty")]
    public required List<ExpenseParticipantDto?> ExpenseParticipant { get; init; }
}
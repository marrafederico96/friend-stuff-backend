using System.ComponentModel.DataAnnotations;

namespace FriendStuff.Features.Expenses.DTOs;

public record class RemoveExpenseParticipantRequest
{
    [Required] public string Username { get; set; } = string.Empty;

    [Required] public string ExpensePublicId { get; set; } = string.Empty;

}

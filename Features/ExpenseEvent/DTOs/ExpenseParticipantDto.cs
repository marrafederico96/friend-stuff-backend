using System.ComponentModel.DataAnnotations;

namespace FriendStuffBackend.Features.ExpenseEvent.DTOs;

public record ExpenseParticipantDto
{
    [Required(ErrorMessage = "Participants cannot be empty")]
    public required string UserName { get; init; }
    public decimal? AmountOwed { get; init; }
    public string? EventName { get; init; }
}
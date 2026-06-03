using System.ComponentModel.DataAnnotations;
using FriendStuff.Domain.Enums;

namespace FriendStuff.Features.Expenses.DTOs;

public record class CreateExpenseRequest
{
    [Required] public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required] public string ActivityPublicId { get; set; } = string.Empty;

    [Required] public string Type { get; set; } = string.Empty;

    [Required] public decimal Amount { get; set; }
}

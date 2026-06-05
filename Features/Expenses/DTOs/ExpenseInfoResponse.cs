namespace FriendStuff.Features.Expenses.DTOs;

public record ExpenseInfoResponse
{
    public required Guid ExpensePublicId { get; set; }
    public string? ExpenseDescription { get; set; }
    public required decimal Amount { get; set; }
    public required string ExpenseName { get; set; }

}
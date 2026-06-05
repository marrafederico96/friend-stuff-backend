using FriendStuff.Features.Expenses.DTOs;

namespace FriendStuff.Features.Activities.DTOs;

public record UserActivityDetailsResponse
{
    public required UserActivityResponse Activity { get; set; }
    public List<ExpenseInfoResponse> Expenses { get; set; } = [];
}

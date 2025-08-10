namespace FriendStuffBackend.Features.ExpenseEvent.DTOs;

public record ResponseBalanceDto
{
    public required decimal BalanceAmount { get; set; }
    public required string DebtorUsername { get; set; }
    public required string PayerUsername { get; set; }
}
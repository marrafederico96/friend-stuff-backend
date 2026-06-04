namespace FriendStuff.Features.UserProfile.DTOs;

public record BalanceResponse
{
    public required int UserId { get; set; }
    public required decimal Amount { get; set; }

}
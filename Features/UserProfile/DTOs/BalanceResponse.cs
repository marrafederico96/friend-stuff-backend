namespace FriendStuff.Features.UserProfile.DTOs;

public record BalanceResponse
{
    public required string Username { get; set; } 
    public required decimal Amount { get; set; }

}
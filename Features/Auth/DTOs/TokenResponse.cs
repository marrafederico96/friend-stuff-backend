
namespace FriendStuff.Features.Auth.DTOs;

public record class TokenResponse
{
    public required string AccessToken { get; set; }

    public string RefreshToken { get; set; } = string.Empty;
}

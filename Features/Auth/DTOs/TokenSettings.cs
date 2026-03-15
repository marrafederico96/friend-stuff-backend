namespace FriendStuff.Features.Auth.DTOs;

public record class TokenSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int ExpirationInMinutes { get; set; }
}

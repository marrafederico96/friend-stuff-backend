using System.Text.Json.Serialization;

namespace FriendStuff.Features.Auth.DTOs;

public record class TokenResponse
{
    public required string AccessToken { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public string RefreshToken { get; set; } = string.Empty;
}

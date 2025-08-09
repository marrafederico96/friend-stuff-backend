namespace FriendStuffBackend.Features.Account.Token.DTOs;

public record TokenDto
{
    public required string AccessToken;
    public required Guid RefreshToken;
};
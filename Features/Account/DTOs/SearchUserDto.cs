namespace FriendStuffBackend.Features.Account.DTOs;

public record SearchUserDto
{
    public required string UserName { get; init; }

}
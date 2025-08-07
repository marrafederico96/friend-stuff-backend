using FriendStuffBackend.Features.UserEvent.DTOs;

namespace FriendStuffBackend.Features.Account.DTOs;

public record UserInfoDto
{
    public required string Email { get; init; }
    public required string UserName { get; init; }
    public List<EventDto?> Events { get; set; } = [];
};
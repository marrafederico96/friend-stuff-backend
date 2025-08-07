using FriendStuffBackend.Domain.Entities.Enum;

namespace FriendStuffBackend.Features.UserEvent.DTOs;

public record EventUserDto
{
    public required string UserName { get; set; }
    public required EventUserRole Role { get; set; }
}
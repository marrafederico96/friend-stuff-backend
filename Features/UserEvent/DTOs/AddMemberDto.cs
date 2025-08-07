namespace FriendStuffBackend.Features.UserEvent.DTOs;

public record AddMemberDto
{
    public required string UserName { get; set; }
    public required string NormalizedEventName { get; set; }
    public required string AdminUsername { get; set; }
}
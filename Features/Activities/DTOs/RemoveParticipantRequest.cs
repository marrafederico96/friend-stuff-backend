namespace FriendStuff.Features.Activities.DTOs;

public record RemoveParticipantRequest
{
    public string Username { get; set; } = string.Empty;
    public string PublicActivityId { get; set; } = string.Empty;
}
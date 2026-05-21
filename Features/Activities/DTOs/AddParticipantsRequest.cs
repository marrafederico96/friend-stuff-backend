using System.ComponentModel.DataAnnotations;

namespace FriendStuff.Features.Activities.DTOs;

public record AddParticipantsRequest
{
    [Required] public List<string> Usernames { get; set; } = [];

    [Required] public string PublicActivityId { get; set; } = string.Empty;
}
using System.ComponentModel.DataAnnotations;

namespace FriendStuff.Features.Activities.DTOs;

public record class AddParticpantsRequest
{
    [Required]
    public List<string> Usernames { get; set; } = [];

    [Required]
    public string PublicActivtyId { get; set; } = string.Empty;
}

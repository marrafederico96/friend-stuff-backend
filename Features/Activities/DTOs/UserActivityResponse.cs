namespace FriendStuff.Features.Activities.DTOs;

public record UserActivityResponse
{

    public required Guid PublicId { get; set; }

    public required string Name { get; set; } = string.Empty;

    public required string? Description { get; set; }

    public required string AdminUsername { get; set; }

    public required string ActivityType { get; set; } = string.Empty;

    public required DateOnly StartDate { get; set; }

    public required DateOnly EndDate { get; set; }
}

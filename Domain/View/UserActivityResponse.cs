
namespace FriendStuff.Domain.View;

public record UserActivityResponse {

    public required Guid PublicId { get; set; }

    public required string Name { get; set; } = string.Empty;

    public required string? Description { get; set; }

    public required  DateOnly StartDate { get; set; }

    public required DateOnly EndDate { get; set; }
}

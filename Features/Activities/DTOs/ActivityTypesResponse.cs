namespace FriendStuff.Features.Activities.DTOs;

public record ActivityTypesResponse {
    public Guid PublicId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
}

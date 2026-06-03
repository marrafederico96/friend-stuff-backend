namespace FriendStuff.Domain.View;

public record ExpenseTypesResponse {
    public string PublicId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
}

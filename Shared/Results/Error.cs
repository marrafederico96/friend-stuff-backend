using FriendStuff.Shared.Results.Enums;

namespace FriendStuff.Shared.Results;

public record class Error
{
    public required string Title { get; init; }
    public required string Message { get; init; }
    public required ErrorType Type { get; init; }
}

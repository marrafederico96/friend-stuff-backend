using FriendStuff.Features.UserProfile.DTOs;
using FriendStuff.Shared.Results;

namespace FriendStuff.Features.UserProfile.Services;

public interface IUserService
{
    public Task<Result<List<BalanceResponse>>> GenerateUserBalance(string username, CancellationToken ct);
}
using System;

namespace FriendStuff.Features.Auth.Services;

public interface ITokenService
{
    public string GenerateAccessToken(string username);
    public Task<string> GenerateRefreshToken(int userId, CancellationToken ct = default);
}

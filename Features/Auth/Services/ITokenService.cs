using System;

namespace FriendStuff.Features.Auth.Services;

public interface ITokenService
{
    public string GenerateAccessToken(string username, string emailAddress);
    public Task<string> GenerateRefreshToken(int userId, CancellationToken ct = default);
}

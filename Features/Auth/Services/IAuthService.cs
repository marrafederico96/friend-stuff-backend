using FriendStuff.Features.Auth.DTOs;
using FriendStuff.Shared.Results;

namespace FriendStuff.Features.Auth.Services;

public interface IAuthService
{
    public Task<Result> AuthRegister(RegisterRequest request, CancellationToken ct = default);
    public Task<Result<TokenResponse>> AuthLogin(LoginRequest request, CancellationToken ct = default);
    public Task<Result> AuthLogout(string refreshTokenValue, CancellationToken cancellationToken);
}

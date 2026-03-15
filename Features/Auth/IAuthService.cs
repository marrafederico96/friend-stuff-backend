using FriendStuff.Features.Auth.DTOs;
using FriendStuff.Shared.Results;

namespace FriendStuff.Features.Auth;

public interface IAuthService
{
    public Task<Result> AuthRegister(RegisterRequest request, CancellationToken ct = default);

}

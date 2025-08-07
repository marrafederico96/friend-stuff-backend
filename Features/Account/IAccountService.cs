using FriendStuffBackend.Features.Auth.DTOs;
using FriendStuffBackend.Features.Auth.Token.DTOs;

namespace FriendStuffBackend.Features.Auth;

/// <summary>
/// Defines authentication-related operations.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user and stores their information in the database.
    /// </summary>
    /// <param name="registerData">An object containing the user's registration details.</param>
    /// <returns>A task that represents the asynchronous registration operation.</returns>
    public Task RegisterUser(RegisterDto registerData);

    /// <summary>
    /// Authenticates a user based on provided login credentials.
    /// </summary>
    /// <param name="loginData">An object containing the user's login credentials.</param>
    /// <returns>A task that represents the asynchronous login operation, returning access and refresh tokens.</returns>
    public Task<TokenDto> LoginUser(LoginDto loginData);

    /// <summary>
    /// Logs out the user by invalidating their active refresh tokens.
    /// </summary>
    /// <param name="userInfoData">An object containing the user's email address.</param>
    /// <returns>A task that represents the asynchronous logout operation.</returns>
    public Task LogoutUser(UserInfoDto userInfoData);
}
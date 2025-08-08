using FriendStuffBackend.Features.Account.DTOs;
using FriendStuffBackend.Features.Account.Token.DTOs;

namespace FriendStuffBackend.Features.Account;

/// <summary>
/// Defines operations related to user authentication and account management.
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Registers a new user and stores their information in the database.
    /// </summary>
    /// <param name="registerData">An object containing the user's registration details.</param>
    /// <returns>A task representing the asynchronous registration operation.</returns>
    public Task RegisterUser(RegisterDto registerData);

    /// <summary>
    /// Authenticates a user based on the provided login credentials.
    /// </summary>
    /// <param name="loginData">An object containing the user's login credentials.</param>
    /// <returns>
    /// A task representing the asynchronous login operation, returning a token object containing access and refresh tokens if successful.
    /// </returns>
    public Task<TokenDto> LoginUser(LoginDto loginData);

    /// <summary>
    /// Retrieves detailed information about a specific user.
    /// </summary>
    /// <param name="email">The email address of the user whose information is being requested.</param>
    /// <returns>
    /// A task representing the asynchronous operation, returning the user's information if found.
    /// </returns>
    public Task<UserInfoDto> GetUserInfo(string email);

    /// <summary>
    /// Deletes a specific user from the system.
    /// </summary>
    /// <param name="email">The email address of the user to delete.</param>
    /// <returns>A task representing the asynchronous deletion operation.</returns>
    public Task DeleteUser(string email);
}

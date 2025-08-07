using FriendStuffBackend.Features.Account.Token.DTOs;

namespace FriendStuffBackend.Features.Account.Token;

/// <summary>
/// Provides functionality for generating, validating, and extracting authentication tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JSON Web Token (JWT) and a refresh token for the specified user.
    /// </summary>
    /// <param name="email">The email address of the user for whom the tokens are being generated.</param>
    /// <returns>
    /// A task representing the asynchronous operation. 
    /// The task result contains a <see cref="TokenDto"/> with the access and refresh tokens.
    /// </returns>
    public Task<TokenDto> GenerateToken(string email);

    /// <summary>
    /// Checks whether a given refresh token is valid and active.
    /// </summary>
    /// <param name="refreshToken">The refresh token to validate.</param>
    /// <returns>
    /// A task representing the asynchronous operation. 
    /// The task result is <c>true</c> if the token is valid; otherwise, <c>false</c>.
    /// </returns>
    public Task<bool> CheckRefreshToken(string refreshToken);

    /// <summary>
    /// Retrieves the email address associated with a given refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to look up.</param>
    /// <returns>
    /// A task representing the asynchronous operation. 
    /// The task result is the email address if the token is valid; otherwise, <c>null</c>.
    /// </returns>
    public Task<string?> GetEmailByRefreshToken(string refreshToken);
}
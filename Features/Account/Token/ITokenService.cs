using FriendStuffBackend.Domain.Entities;
using FriendStuffBackend.Features.Auth.Token.DTOs;

namespace FriendStuffBackend.Features.Auth.Token;

/// <summary>
/// Provides functionality for generating authentication tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JSON Web Token (JWT) and a refresh token for the specified user.
    /// </summary>
    /// <param name="user">The user entity for whom the token is being generated.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains a <see cref="TokenDto"/> with the access and refresh tokens.</returns>
    Task<TokenDto> GenerateToken(User user);
}

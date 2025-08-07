using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using FriendStuffBackend.Data;
using FriendStuffBackend.Domain.Entities;
using FriendStuffBackend.Features.Auth.Token.DTOs;
using Microsoft.IdentityModel.Tokens;

namespace FriendStuffBackend.Features.Auth.Token;

public class TokenService(FriendStuffDbContext context) : ITokenService
{
    public async Task<TokenDto> GenerateToken(User user)
    {
        // Define claims for the user (username and role)
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, "USER")
        };
        var claimsIdentity = new ClaimsIdentity(claims, "Bearer");

        // Load the RSA private key from PEM file
        const string rsaPrivateKeyPath = "./Certs/private.pem";
        var rsaPrivateKey = await File.ReadAllTextAsync(rsaPrivateKeyPath);

        // Create an RSA security key from the PEM content
        var rsa = RSA.Create();
        rsa.ImportFromPem(rsaPrivateKey);
        var securityKey = new RsaSecurityKey(rsa);

        // Describe the token parameters
        SecurityTokenDescriptor securityTokenDescriptor = new()
        {
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddHours(1), // Token valid for 1 hour
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256)
        };

        // Generate the JWT access token
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(securityTokenDescriptor);

        // Create and return the token DTO with access and refresh tokens
        TokenDto tokenData = new()
        {
            AccessToken = tokenHandler.WriteToken(token),
            RefreshToken = await GenerateRefreshToken(user)
        };

        return tokenData;
    }

    private async Task<Guid> GenerateRefreshToken(User user)
    {
        // Invalidate all previous refresh tokens for the user
        user.RefreshTokens?.ToList().ForEach(t => t.IsValid = false);

        // Create a new refresh token
        RefreshToken refreshToken = new()
        {
            TokenValue = Guid.NewGuid(),
            UserId = user.Id,
            IsValid = true,
            ExpireAt = DateTime.UtcNow.AddDays(15), // Refresh token valid for 15 days
            CreatedAt = DateTime.UtcNow
        };

        // Persist the new refresh token in the database
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();

        return refreshToken.TokenValue;
    }
}
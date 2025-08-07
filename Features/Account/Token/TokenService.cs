using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using FriendStuffBackend.Data;
using FriendStuffBackend.Domain.Entities;
using FriendStuffBackend.Features.Account.Token.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FriendStuffBackend.Features.Account.Token;

public class TokenService(FriendStuffDbContext context) : ITokenService
{
    public async Task<TokenDto> GenerateToken(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await context.Users
                       .Where(user => user.Email == normalizedEmail)
                       .Include(user => user.RefreshTokens)
                       .FirstOrDefaultAsync() ??
                   throw new ArgumentException("User not found");
        // Define claims for the user (username and role)
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.NormalizedUserName),
            new(ClaimTypes.Email, normalizedEmail),
            new(ClaimTypes.Role, "USER")
        };
        var claimsIdentity = new ClaimsIdentity(claims, "Bearer");

        var rsaPrivateKey = Environment.GetEnvironmentVariable("private_key");
        if (string.IsNullOrWhiteSpace(rsaPrivateKey))
        {
            throw new InvalidOperationException("La variabile PRIVATE_KEY non è impostata.");
        }

        // Create an RSA security key from the PEM content
        var rsa = RSA.Create();
        rsa.ImportFromPem(rsaPrivateKey);
        var securityKey = new RsaSecurityKey(rsa);

        // Describe the token parameters
        SecurityTokenDescriptor securityTokenDescriptor = new()
        {
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddMinutes(15), // Token valid for 15 minutes
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

    public async Task<bool> CheckRefreshToken(string refreshToken) {
            var token = await context.RefreshTokens
                .Where(t => t.TokenValue.ToString() == refreshToken.Trim())
                .FirstOrDefaultAsync();

            if (token == null)
                return false;

            return token.IsValid && token.ExpireAt >= DateTime.UtcNow;
    }

    public async Task<string?> GetEmailByRefreshToken(string refreshToken)
    {
        var token = await context.RefreshTokens
            .Where(t => t.TokenValue.ToString() == refreshToken.Trim())
            .Include(t => t.User)
            .FirstOrDefaultAsync() ?? throw new ArgumentException("Token not found");

        return token.User?.Email;
    }

}

using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FriendStuff.Data;
using FriendStuff.Domain.Entities;
using FriendStuff.Features.Auth.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FriendStuff.Features.Auth.Services;

public class TokenService(IOptions<TokenSettings> options, FriendStuffDbContext context) : ITokenService
{
    public string GenerateAccessToken(string username, string emailAddress)
    {
        var tokenSettings = options.Value;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Name, username),
                new Claim(JwtRegisteredClaimNames.Email,emailAddress)
            ]),

            Issuer = tokenSettings.Issuer,
            Audience = tokenSettings.Audience,
            Expires = DateTime.UtcNow.AddMinutes(tokenSettings.ExpirationInMinutes),
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(tokenDescriptor);

    }

    public async Task<string> GenerateRefreshToken(int userId, CancellationToken ct)
    {
        await InvalidOldRefrehTokens(userId, ct);

        var tokenValue = Guid.NewGuid().ToString();
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(tokenValue)));

        RefreshToken newRefreshToken = new()
        {
            UserId = userId,
            TokenHash = tokenHash
        };

        context.RefreshTokens.Add(newRefreshToken);
        await context.SaveChangesAsync(ct);

        return tokenValue;

    }

    private async Task InvalidOldRefrehTokens(int userId, CancellationToken ct) => await context.RefreshTokens
        .Where(rt => rt.UserId == userId)
        .ExecuteUpdateAsync(setters => setters.SetProperty(rt => rt.Valid, false), cancellationToken: ct);
}

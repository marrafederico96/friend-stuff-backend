using System;
using System.Security.Claims;
using System.Text;
using FriendStuff.Features.Auth.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FriendStuff.Features.Auth.Services;

public class TokenService(IOptions<TokenSettings> options) : ITokenService
{
    public string GenerateAccessToken(string username)
    {
        var tokenSettings = options.Value;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, username)
            ]),
            Issuer = tokenSettings.Issuer,
            Audience = tokenSettings.Audience,
            Expires = DateTime.UtcNow.AddMinutes(tokenSettings.ExpirationInMinutes),
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(tokenDescriptor);

    }
}

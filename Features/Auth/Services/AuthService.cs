using System;
using FriendStuff.Data;
using FriendStuff.Features.Auth.DTOs;
using FriendStuff.Shared.Results;
using Microsoft.EntityFrameworkCore;
using FriendStuff.Shared.Results.Enums;
using FriendStuff.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace FriendStuff.Features.Auth.Services;

public class AuthService(FriendStuffDbContext context, IPasswordHasher<User> passwordHasher, ITokenService tokenService) : IAuthService
{

    public async Task<Result> AuthRegister(RegisterRequest request, CancellationToken ct = default)
    {
        var normalizedUsername = request.Username.Trim().ToUpperInvariant();
        var normalizedEmail = request.EmailAddress.Trim().ToUpperInvariant();

        var checkEmail = await context.Users.AnyAsync(u => u.NormalizedEmailAddress == normalizedEmail, cancellationToken: ct);
        if (checkEmail)
            return Result.Failure(new Error
            {
                Title = "Auth register error",
                Message = "Email already exists",
                Type = ErrorType.Conflict
            });

        var checkUsername = await context.Users.AnyAsync(u => u.NormalizedUsername == normalizedUsername, cancellationToken: ct);
        if (checkUsername)
            return Result.Failure(new Error
            {
                Title = "Auth register error",
                Message = "Username already exists",
                Type = ErrorType.Conflict
            });

        var newUser = new User
        {
            Username = request.Username,
            EmailAddress = request.EmailAddress,
            NormalizedUsername = request.Username.Trim().ToUpperInvariant(),
            NormalizedEmailAddress = request.EmailAddress.Trim().ToUpperInvariant(),
            PasswordHash = ""
        };
        newUser.PasswordHash = passwordHasher.HashPassword(newUser, request.Password);

        context.Users.Add(newUser);
        await context.SaveChangesAsync(ct);

        return Result.Success("User registered");

    }

    public async Task<Result<TokenResponse>> AuthLogin(LoginRequest request, CancellationToken ct = default)
    {
        var normalizedEmail = request.EmailAddress.Trim().ToUpperInvariant();

        var user = await context.Users
            .AsNoTracking()
            .Where(u => u.NormalizedEmailAddress == normalizedEmail)
            .FirstOrDefaultAsync(cancellationToken: ct);

        if (user == null)
            return Result<TokenResponse>.Failure(new Error
            {
                Title = "Auth login error",
                Message = "Wrong credentials",
                Type = ErrorType.Unauthorized
            });

        var checkPwd = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (checkPwd == PasswordVerificationResult.Failed)
            return Result<TokenResponse>.Failure(new Error
            {
                Title = "Auth login error",
                Message = "Wrong credentials",
                Type = ErrorType.Unauthorized
            });

        // Genero JWT e Refresh Token
        var jwt = tokenService.GenerateAccessToken(user.Username, user.EmailAddress);
        var refreshTokenValue = await tokenService.GenerateRefreshToken(user.Id, ct);

        var response = new TokenResponse
        {
            AccessToken = jwt,
            RefreshToken = refreshTokenValue
        };

        return Result<TokenResponse>.Success(response, "User logged in");

    }

    public async Task<Result> AuthLogout(string refreshTokenValue, CancellationToken cancellationToken)
    {
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshTokenValue)));

        var rowUpdate = await context.RefreshTokens
            .Where(t => t.TokenHash == tokenHash)
            .ExecuteUpdateAsync(setters => setters.SetProperty(t => t.Valid, false), cancellationToken: cancellationToken);

        if (rowUpdate == 0)
            return Result.Failure(new Error
            {
                Title = "Auth error",
                Message = "Refreh token not found",
                Type = ErrorType.NotFound
            });

        return Result.Success();
    }

    public async Task<Result<TokenResponse>> AuthRefresh(string refreshTokenValue, CancellationToken ct = default)
    {
        var refreshTokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshTokenValue)));

        var checkHashValue = await context.RefreshTokens
            .AnyAsync(ct => ct.TokenHash == refreshTokenHash && ct.Valid == true && ct.ExpireAt >= DateTime.UtcNow, cancellationToken: ct);

        if (checkHashValue == false)
            return Result<TokenResponse>.Failure(new Error
            {
                Title = "Auth refresh error",
                Message = "Refreh token not valid",
                Type = ErrorType.Unauthorized
            });

        await context.RefreshTokens
            .Where(rt => rt.TokenHash == refreshTokenHash)
            .ExecuteUpdateAsync(setters => setters.SetProperty(rt => rt.Valid, false), cancellationToken: ct);


        var userId = await context.RefreshTokens
                    .Where(rt => rt.TokenHash == refreshTokenHash)
                    .Select(rt => rt.UserId)
                    .FirstOrDefaultAsync(cancellationToken: ct);

        var userData = await context.Users
            .Where(u => u.Id == userId)
            .Select(u => new { username = u.Username, emailAddress = u.EmailAddress })
            .FirstOrDefaultAsync(cancellationToken: ct) ?? throw new ArgumentException("Error. User not found");

        var accessToken = tokenService.GenerateAccessToken(userData.username, userData.emailAddress);
        var refreshToken = await tokenService.GenerateRefreshToken(userId, ct);

        var tokenResponse = new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };

        return Result<TokenResponse>.Success(tokenResponse);

    }
}

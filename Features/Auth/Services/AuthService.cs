using System;
using FriendStuff.Data;
using FriendStuff.Features.Auth.DTOs;
using FriendStuff.Shared.Results;
using Microsoft.EntityFrameworkCore;
using FriendStuff.Shared.Results.Enums;
using FriendStuff.Domain.Entities;
using Microsoft.AspNetCore.Identity;

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

        var newUser = User.Create(request.Username, request.EmailAddress, request.Password, passwordHasher);
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
        var jwt = tokenService.GenerateAccessToken(user.Username);
        var refreshTokenValue = await tokenService.GenerateRefreshToken(user.Id, ct);

        var response = new TokenResponse
        {
            AccessToken = jwt,
            RefreshToken = refreshTokenValue
        };

        return Result<TokenResponse>.Success(response, "User logged in");

    }
}

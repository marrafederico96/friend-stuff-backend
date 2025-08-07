using FriendStuffBackend.Data;
using FriendStuffBackend.Domain.Entities;
using FriendStuffBackend.Features.Auth.DTOs;
using FriendStuffBackend.Features.Auth.Token;
using FriendStuffBackend.Features.Auth.Token.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FriendStuffBackend.Features.Auth;

public class AuthService(FriendStuffDbContext context, IPasswordHasher<User> passwordHasher, ITokenService tokenService) : IAuthService
{
    // Registers a new user in the system
    public async Task RegisterUser(RegisterDto registerData)
    {
        // Normalize the email and username: remove leading/trailing whitespace and convert to lowercase
        var normalizedEmail = registerData.Email.Trim().ToLowerInvariant();
        var normalizedUsername = registerData.UserName.Trim().ToLowerInvariant();

        // Check if a user already exists with the same normalized email or username
        var user = await context.Users
            .AnyAsync(user => user.Email == normalizedEmail || user.NormalizedUserName == normalizedUsername);

        if (user)
        {
            // If user already exists, throw an exception
            throw new ArgumentException("User already exists");
        }
    
        // Create a new User entity with the provided registration information
        var newUser = new User
        {
            UserName = registerData.UserName.Trim(),
            NormalizedUserName = normalizedUsername,
            Email = normalizedEmail,
            CreatedAt = DateTime.UtcNow
        };

        // Hash the user's password securely using the password hasher
        newUser.PasswordHash = passwordHasher.HashPassword(newUser, registerData.Password);
    
        // Add the new user to the database
        await context.Users.AddAsync(newUser);
        await context.SaveChangesAsync(); // Save changes to persist the new user
    }

    // Logs a user in and returns a token
    public async Task<TokenDto> LoginUser(LoginDto loginData)
    {
        var normalizedEmail = loginData.Email.Trim().ToLowerInvariant();

        // Look for the user with the provided email, including their refresh tokens
        var user = await context.Users
            .Where(user => user.Email == normalizedEmail)
            .Include(user => user.RefreshTokens)
            .FirstOrDefaultAsync() ?? throw new ArgumentException("Wrong credentials");
        
        // Verify that the password matches the hashed password stored in the database
        var check = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginData.Password);
        if (check == PasswordVerificationResult.Failed)
        {
            // If the password is incorrect, throw an exception
            throw new ArgumentException("Wrong credentials");
        }

        // Generate a new access token and refresh token for the user
        var tokenData = await tokenService.GenerateToken(user);
        TokenDto accessToken = new()
        {
            AccessToken = tokenData.AccessToken,
            RefreshToken = tokenData.RefreshToken
        };
        return accessToken; // Return the token to the client
    }

    // Logs the user out by invalidating all their refresh tokens
    public async Task LogoutUser(UserInfoDto userInfoData)
    {
        var normalizedEmail = userInfoData.Email?.Trim().ToLowerInvariant();

        // Find the user by email, including their refresh tokens
        var user = await context.Users
            .Where(user => user.Email == normalizedEmail)
            .Include(user => user.RefreshTokens)
            .FirstOrDefaultAsync() ?? throw new ArgumentException("User not found");
        
        // Mark all refresh tokens as invalid
        user.RefreshTokens?.ToList().ForEach(t => t.IsValid = false);
        
        // Save changes to update the token statuses in the database
        await context.SaveChangesAsync();
    }
}

using FriendStuffBackend.Data;
using FriendStuffBackend.Domain.Entities;
using FriendStuffBackend.Features.Account.DTOs;
using FriendStuffBackend.Features.Account.Token;
using FriendStuffBackend.Features.Account.Token.DTOs;
using FriendStuffBackend.Features.ExpenseEvent.DTOs;
using FriendStuffBackend.Features.UserEvent.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FriendStuffBackend.Features.Account;

public class AccountService(FriendStuffDbContext context, IPasswordHasher<User> passwordHasher, ITokenService tokenService) : IAccountService
{
    // Registers a new user in the system
    public async Task RegisterUser(RegisterDto registerData)
    {
        // Normalize the email and username: remove leading/trailing whitespace and convert to lowercase
        var normalizedEmail = registerData.Email.Trim().ToLowerInvariant();
        var normalizedUsername = registerData.UserName.Trim().ToLowerInvariant();

        // Check if a user already exists with the same normalized email or username
        var userExists = await context.Users
            .AnyAsync(user => user.Email == normalizedEmail || user.NormalizedUserName == normalizedUsername);

        if (userExists)
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
        // Normalize the email: remove leading/trailing whitespace and convert to lowercase
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
        var tokenData = await tokenService.GenerateToken(user.Email);
        return tokenData; // Return the token to the client
    }

    // Logs the user out by invalidating all their refresh tokens
    public async Task LogoutUser(string email)
    {
        // Normalize the email: remove leading/trailing whitespace and convert to lowercase
        var normalizedEmail = email.Trim().ToLowerInvariant();

        // Find the userName by userNameName, including their refresh tokens
        var user = await context.Users
            .Where(user => user.Email == normalizedEmail)
            .Include(user => user.RefreshTokens)
            .FirstOrDefaultAsync() ?? throw new ArgumentException("User not found");
        
        // Mark all refresh tokens as invalid
        user.RefreshTokens.ToList().ForEach(t => t.IsValid = false);
        // Save changes to update the token statuses in the database
        await context.SaveChangesAsync();
    }

    public async Task<UserInfoDto> GetUserInfo(string email)
    {
        // Normalize the email: remove leading/trailing whitespace and convert to lowercase
        var normalizedEmail = email.Trim().ToLowerInvariant();

        // Find the user by email
        var user = await context.Users
                       .Where(u => u.Email == normalizedEmail)

                       .Include(u => u.Events)
                       .ThenInclude(eu => eu.Event)

                       .Include(u => u.Events)
                       .ThenInclude(eu => eu.Event)
                       .ThenInclude(e => e.Participants)
                       .ThenInclude(ep => ep.Participant)

                       .Include(u => u.Events)
                       .ThenInclude(eu => eu.Event)
                       .ThenInclude(e => e.Admin)

                       .Include(u => u.Events)
                       .ThenInclude(eu => eu.Event)
                       .ThenInclude(e => e.Expenses)

                       .Include(u => u.Events)
                       .ThenInclude(eu => eu.Event)
                       .ThenInclude(e => e.Expenses)
                       .ThenInclude(exp => exp.Payer)

                       .Include(u => u.Events)
                       .ThenInclude(eu => eu.Event)
                       .ThenInclude(e => e.Expenses)
                       .ThenInclude(exp => exp.Participants)
                       .ThenInclude(expPart => expPart.Participant)

                       .Include(u => u.ExpenseParticipants)
                       .ThenInclude(expPart => expPart.Expense)

                       .Include(u => u.ExpensesPayed)

                       .FirstOrDefaultAsync()
                   ?? throw new ArgumentException("User not found");


        var listEvent = user.Events
            .Select(eu =>
            {
                if (eu.Event is { Admin: not null })
                    return new EventDto
                    {
                        EventName = eu.Event.EventName,
                        NormalizedEventName = eu.Event.NormalizedEventName,
                        StartDate = eu.Event.StartDate,
                        EndDate = eu.Event.EndDate,
                        AdminEmail = eu.Event.Admin.Email,
                        Participants = eu.Event.Participants.Select(p =>
                        {
                            if (p.Participant != null)
                                return new EventUserDto
                                {
                                    UserName = p.Participant.UserName,
                                    Role = p.UserRole
                                };
                            return null;
                        }).ToList(),
                        ExpensesEvent = eu.Event.Expenses
                            .OrderByDescending(ex => ex.ExpenseDate)
                            .Select(ex =>
                            {
                                if (ex.Payer != null)
                                    return new ExpenseEventDto
                                    {
                                        EventName = ex.ExpenseName,
                                        Amount = ex.Amount,
                                        PayerUsername = ex.Payer.NormalizedUserName,
                                        ExpenseName = ex.ExpenseName,
                                        ExpenseParticipant = ex.Participants
                                            .Select(p =>
                                            {
                                                if (p.Participant?.UserName != null)
                                                    return new ExpenseParticipantDto
                                                    {
                                                        UserName = p.Participant.UserName,
                                                    };
                                                return null;
                                            }).ToList()
                                    };
                                return null;
                            }).ToList()
                    };
                return null;
            })
            .ToList();

        UserInfoDto userInfo = new()
        {
            Email = user.Email,
            UserName = user.UserName,
            Events = listEvent
        };
        return userInfo;
    }

    public async Task DeleteUser(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail) ?? throw new ArgumentException("User not found");

        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }
}

using FriendStuff.Data;
using FriendStuff.Features.UserProfile.DTOs;
using FriendStuff.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace FriendStuff.Features.UserProfile.Services;

public class UserService(FriendStuffDbContext context) : IUserService
{
    public async Task<Result<List<BalanceResponse>>> GenerateUserBalance(string username, CancellationToken ct)
    {
        var normalizedUsername = username.Trim().ToUpperInvariant();

        var userId = await context.Users
            .Where(u => u.NormalizedUsername == normalizedUsername)
            .Select(u => u.Id)
            .FirstOrDefaultAsync(ct);

        if (userId == default)
        {
            return Result<List<BalanceResponse>>.Failure(new Error
            {
                Title = "Balance error",
                Message = "User not found",
                Type = Shared.Results.Enums.ErrorType.NotFound
            });
        }

        var balancesFromDb = await context.UsersExpenses
            .Where(ue => ue.Expense != null && ((ue.Expense.PayerId == userId && ue.DebtorId != userId) ||
                         (ue.DebtorId == userId && ue.Expense.PayerId != userId)))
            .Select(ue => new
            {
                OtherUserId = ue.Expense!.PayerId == userId ? ue.DebtorId : ue.Expense!.PayerId,
                Credit = ue.Expense!.PayerId == userId ? ue.AmountOwed : 0,
                Debt = ue.DebtorId == userId ? ue.AmountOwed : 0
            })
            .GroupBy(x => x.OtherUserId)
            .Select(g => new
            {
                UserId = g.Key,
                TotalBalance = g.Sum(x => x.Credit) - g.Sum(x => x.Debt)
            })
            .ToListAsync(ct);

        var otherUserIds = balancesFromDb.Select(b => b.UserId).ToList();

        var usernames = await context.Users
            .Where(u => otherUserIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Username, cancellationToken: ct);

        var finalBalances = balancesFromDb.Select(b => new BalanceResponse
        {
            Username = usernames.GetValueOrDefault(b.UserId, "Unknown"),
            Amount = b.TotalBalance
        }).ToList();

        return Result<List<BalanceResponse>>.Success(finalBalances);
    }

    public async Task<Result<decimal>> GetPersonalBalance(string username, CancellationToken ct)
    {
        var normalizedUsername = username.Trim().ToUpperInvariant();

        var userId = await context.Users
            .Where(u => u.NormalizedUsername == normalizedUsername)
            .Select(u => u.Id)
            .FirstOrDefaultAsync(ct);

        if (userId == default)
        {
            return Result<decimal>.Failure(new Error
            {
                Title = "Balance error",
                Message = "User not found",
                Type = Shared.Results.Enums.ErrorType.NotFound
            });
        }

        decimal personalBalance = await context.UsersExpenses
            .Where(ue => ue.DebtorId == userId && ue.Expense != null && ue.Expense.Participants.Count() == 1)
            .SumAsync(ue => ue.AmountOwed, cancellationToken: ct);

        return Result<decimal>.Success(personalBalance);
    }
}
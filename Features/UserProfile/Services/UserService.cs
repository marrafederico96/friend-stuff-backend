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
            .Select(u => u.Id).FirstOrDefaultAsync(ct);

        var payerExpenses = await context.Expenses
            .Where(e => e.PayerId == userId)
            .Select(e => new { e.Id, e.Amount })
            .ToListAsync(ct);

        var debtorExpenses = await context.UsersExpenses
            .Where(e => e.DebtorId == userId && !payerExpenses.Select(e => e.Id).Contains(e.ExpenseId))
            .Include(e => e.Expense)
            .ToListAsync(ct);

        var totalBalance = payerExpenses.Sum(e => e.Amount) - debtorExpenses.Sum(e => e.AmountOwed);

        var userDebtsToMe = await context.UsersExpenses
            .Where(e => payerExpenses.Select(e => e.Id).Contains(e.ExpenseId) && e.DebtorId != userId)
            .GroupBy(e => e.DebtorId)
            .Select(g => new
            {
                UserId = g.Key,
                Amount = g.Sum(e => e.AmountOwed)
            })
            .ToListAsync(ct);


        var myDebtsToOthers = debtorExpenses
            .GroupBy(e => e.Expense!.PayerId)
            .Select(g => new
            {
                UserId = g.Key,
                Amount = g.Sum(e => e.AmountOwed)
            }).ToList();

        var allUserIds = userDebtsToMe.Select(x => x.UserId)
            .Union(myDebtsToOthers.Select(x => x.UserId))
            .ToList();

        var usernames = await context.Users
            .Where(u => allUserIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Username, cancellationToken: ct);

        var finalBalances = allUserIds.Select(id =>
        {
            var credit = userDebtsToMe.FirstOrDefault(x => x.UserId == id)?.Amount ?? 0;
            var debt = myDebtsToOthers.FirstOrDefault(x => x.UserId == id)?.Amount ?? 0;

            return new BalanceResponse
            {
                Username = usernames[id],
                Amount = credit - debt
            };
        }).ToList();


        return Result<List<BalanceResponse>>.Success(finalBalances);
    }
}
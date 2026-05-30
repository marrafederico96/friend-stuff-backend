using FriendStuff.Data;
using FriendStuff.Domain.Entities;
using FriendStuff.Features.Expenses.DTOs;
using FriendStuff.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace FriendStuff.Features.Expenses.Services;

public class ExpenseService(FriendStuffDbContext context) : IExpenseService
{

    public async Task<Result> CreateExpense(CreateExpenseRequest request, string payerUsername, CancellationToken ct)
    {
        var normalizedPayerUsername = payerUsername.Trim().ToUpperInvariant();

        var payerId = await context.Users
            .Where(p => p.NormalizedUsername == normalizedPayerUsername)
            .Select(p => p.Id)
            .FirstOrDefaultAsync(ct);

        _ = Guid.TryParse(request.ActivityPublicId, out var activityPublicId);
        var activityId = await context.Activities
            .Where(a => a.PublicId == activityPublicId)
            .Select(a => a.Id)
            .FirstOrDefaultAsync(ct);

        var newExpense = new Expense
        {
            ActivityId = activityId,
            Amount = request.Amount,
            Name = request.Name,
            Description = request.Description,
            PayerId = payerId,
            Type = request.Type,
            Participants = 
            [
                new UserExpense 
                { 
                    AmountOwed = request.Amount
                }
            ]
        };

        context.Expenses.Add(newExpense);
        await context.SaveChangesAsync(ct);

        return Result.Success("Expense created");
    }

    public async Task<Result> AddExpenseParticipant(AddExpenseParticipantRequest request, CancellationToken ct)
    {

        var normalizedUsernames = request.Usernames
           .Select(u => u.Trim().ToUpperInvariant())
           .ToList();

        var activityId = await context.Activities
            .AsNoTracking()
            .Where(a => a.PublicId.ToString() == request.PublicActivityId)
            .Select(a => a.Id)
            .FirstAsync(ct);

        var expenseData = await context.Expenses
            .Where(e => e.PublicId.ToString() == request.PublicExpenseId)
            .Select(e => new { e.Id, e.Amount})
            .FirstOrDefaultAsync(ct);

        if (expenseData == null)
            return Result.Failure(new Error
            {
                Title = "Add Expense participant error",
                Message = "Expense not found",
                Type = Shared.Results.Enums.ErrorType.Forbidden
            });

        var userIds = await context.Users
           .AsNoTracking()
           .Where(u => normalizedUsernames.Contains(u.NormalizedUsername))
           .Select(u => u.Id)
           .ToListAsync(ct);

        var countParticipants = await context.UsersExpenses.CountAsync(e => e.Id == expenseData.Id, cancellationToken: ct);

        var newUserExpenseParticipants = userIds.Select(u => new UserExpense
        {
            AmountOwed = expenseData.Amount / countParticipants,
            DebtorId = u,
            ExpenseId = expenseData.Id
        });

        context.UsersExpenses.AddRange(newUserExpenseParticipants);
        await context.SaveChangesAsync(ct);
        
        return Result.Success("Expense Participants added");
    }
}
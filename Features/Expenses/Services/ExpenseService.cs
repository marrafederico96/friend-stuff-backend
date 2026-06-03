using FriendStuff.Data;
using FriendStuff.Domain.Entities;
using FriendStuff.Domain.View;
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

        var expenseTypeId = await context.ExpenseTypes
            .Where(t => t.NormalizedName == request.Type.Trim().ToUpperInvariant())
            .Select(t => t.Id)
            .FirstOrDefaultAsync();

        var newExpense = new Expense
        {
            ActivityId = activityId,
            Amount = request.Amount,
            Name = request.Name,
            Description = request.Description,
            PayerId = payerId,
            TypeId = expenseTypeId,
            Participants =
            [
                new UserExpense
                {
                    AmountOwed = request.Amount,
                    DebtorId = payerId
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
            .Select(e => new { e.Id, e.Amount, e.PayerId})
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

        foreach(var id in userIds) {
            var checkId = await context.UsersActivities.Where(ua => ua.ActivityId == activityId && ua.UserId == id).Select(ua => ua.UserId).FirstOrDefaultAsync();
            if (checkId == 0)
                return Result.Failure(new Error
                {
                    Title = "Expense error",
                    Message = "Expense participants not present in activity",
                    Type = Shared.Results.Enums.ErrorType.Forbidden
                });
        }

        var participantsCount = await context.UsersExpenses.CountAsync(ue => ue.ExpenseId == expenseData.Id);
        var totalParticipants = userIds.Count + participantsCount;

        var newUserExpenseParticipants = userIds.Select(debtorId => new UserExpense
        {
            AmountOwed = expenseData.Amount / totalParticipants,
            DebtorId = debtorId,
            ExpenseId = expenseData.Id
        });

        await context.UsersExpenses
            .Where(ue => ue.ExpenseId == expenseData.Id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.AmountOwed,expenseData.Amount / totalParticipants));

        context.UsersExpenses.AddRange(newUserExpenseParticipants);
        await context.SaveChangesAsync(ct);

        return Result.Success("Expense Participants added");
    }

    public async Task<Result<List<ExpenseTypesResponse>>> GetExpenseTypes()
    {

        var expenseTypes = await context.ExpenseTypesResponse
            .Select(at => new ExpenseTypesResponse
            {
                Name = at.Name,
                NormalizedName = at.NormalizedName,
                PublicId = at.PublicId.ToString()
            })
            .ToListAsync();

        return Result<List<ExpenseTypesResponse>>.Success(expenseTypes);
    }
}

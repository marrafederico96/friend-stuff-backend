using System;
using FriendStuff.Data;
using FriendStuff.Domain.Entities;
using FriendStuff.Features.Expenses.DTOs;
using FriendStuff.Shared.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FriendStuff.Features.Expenses.Services;

public class ExpenseService(FriendStuffDbContext context) : IExpenseService
{
    public async Task<Result> CreateExpense(CreateExpenseRequest request, string payerUsername, CancellationToken ct)
    {
        var normalizedPayerUsername = payerUsername.Trim().ToUpperInvariant();

        var payerId = await context.Users
            .Where(p => p.NormalizedUsername == normalizedPayerUsername)
            .Select(p => p.Id)
            .FirstOrDefaultAsync(cancellationToken: ct);

        _ = Guid.TryParse(request.ActivityPublicId, out var activityPublicId);
        var activityId = await context.Activities
            .Where(a => a.PublicId == activityPublicId)
            .Select(a => a.Id)
            .FirstOrDefaultAsync(cancellationToken: ct);

        var newExpense = new Expense
        {
            ActivityId = activityId,
            Amount = request.Amount,
            Name = request.Name,
            Description = request.Descritpion,
            PayerId = payerId,
            Type = request.Type,
        };

        context.Expenses.Add(newExpense);
        await context.SaveChangesAsync(ct);

        return Result.Success("Expense created");
    }
}

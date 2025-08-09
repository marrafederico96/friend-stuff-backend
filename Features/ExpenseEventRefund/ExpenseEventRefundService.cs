using FriendStuffBackend.Data;
using FriendStuffBackend.Domain.Entities;
using FriendStuffBackend.Features.ExpenseEventRefund.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FriendStuffBackend.Features.ExpenseEventRefund;

public class ExpenseEventRefundService(FriendStuffDbContext context) : IExpenseEventRefundService
{
    public async Task AddRefund(ExpenseEventRefundDto refundData)
    {
        var normalizedDebtorUsername = refundData.DebtorUsername.Trim().ToLowerInvariant();
        var normalizedPayerUsername = refundData.PayerUsername.Trim().ToLowerInvariant();
        var normalizedExpenseName = refundData.ExpenseName.TrimEnd().TrimStart().ToLowerInvariant();

        var debtor = await context.Users
            .Where(u => u.NormalizedUserName == normalizedDebtorUsername)
            .Include(u => u.ExpenseParticipants)
            .ThenInclude(ex => ex.Expense)
            .FirstOrDefaultAsync() ?? throw new ArgumentException("Debtor not found");
        
        var payer =  await context.Users
            .Where(u => u.NormalizedUserName == normalizedPayerUsername)
            .Include(u => u.ExpenseParticipants)
            .ThenInclude(ex => ex.Expense )
            .FirstOrDefaultAsync() ?? throw new ArgumentException("Payer not found");

        var amountOwed = payer.ExpenseParticipants
            .Select(ex => ex.AmountOwed).Single();

        if (Math.Round(refundData.AmountRefund,2) >= amountOwed)
        {
            throw new ArgumentException("Refund must be greater than amount owed");
        }

        var currentExpense = payer.ExpenseParticipants
                                 .FirstOrDefault(ex =>
                                     ex.Expense != null && ex.Expense.ExpenseName == normalizedExpenseName)
                             ?? throw new ArgumentException("Expense not found");
        
        ExpenseRefund newRefund = new()
        {
             AmountRefund = Math.Round(refundData.AmountRefund,2),
             DebtorId = debtor.Id,
             ExpenseId = currentExpense.ExpenseId,
             RefundDate = DateTime.UtcNow
        };

        var updateAmountOwed = amountOwed - refundData.AmountRefund;

        await context.ExpenseRefunds.AddAsync(newRefund);
        await context.SaveChangesAsync();
    }
}
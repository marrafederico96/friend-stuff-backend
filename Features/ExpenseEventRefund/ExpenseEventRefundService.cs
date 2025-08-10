using FriendStuffBackend.Data;
using FriendStuffBackend.Domain.Entities;
using FriendStuffBackend.Features.ExpenseEvent.DTOs;
using FriendStuffBackend.Features.ExpenseEventRefund.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FriendStuffBackend.Features.ExpenseEventRefund;

public class ExpenseEventRefundService(FriendStuffDbContext context) : IExpenseEventRefundService
{
    public async Task AddRefund(ExpenseEventRefundDto refundData)
    {
        var normalizedDebtorUsername = refundData.DebtorUsername.Trim().ToLowerInvariant();
        var normalizedPayerUsername = refundData.PayerUsername.Trim().ToLowerInvariant();

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

        var balance = await GetBalanceBetweenUsers(debtor.Id, payer.Id);

        if (refundData.AmountRefund > balance)
        {
            throw new ArgumentException("Refund cannot exceed current balance owed.");
        }

        if (refundData.AmountRefund <= 0)
        {
            throw new ArgumentException("Refund must be positive.");
        }

        ExpenseRefund newRefund = new()
        {
             AmountRefund = Math.Round(refundData.AmountRefund,2),
             DebtorId = debtor.Id,
             PayerId = payer.Id,
             RefundDate = DateTime.UtcNow
        };

        await context.ExpenseRefunds.AddAsync(newRefund);
        await context.SaveChangesAsync();
    }
    
    private async Task<decimal> GetBalanceBetweenUsers(Guid debtorId, Guid payerId)
    {
        var debtor = await context.Users
            .Where(u => u.Id == debtorId)
            .Include(u => u.ExpenseParticipants).ThenInclude(ep => ep.Expense)
            .Include(u => u.ExpenseAsDebtor)
            .FirstOrDefaultAsync() ?? throw new ArgumentException("Debtor not found");

        var payer = await context.Users
            .Where(u => u.Id == payerId)
            .Include(u => u.ExpenseParticipants).ThenInclude(ep => ep.Expense)
            .Include(u => u.ExpenseAsDebtor)
            .FirstOrDefaultAsync() ?? throw new ArgumentException("Payer not found");

        var amountOwedToOther = debtor.ExpenseParticipants
            .Where(ep => ep.Expense != null && ep.Expense.PayerId == payer.Id)
            .Sum(ep => ep.AmountOwed);

        var refundToOther = debtor.ExpenseAsDebtor
            .Where(er => er.PayerId == payer.Id)
            .Sum(er => er.AmountRefund);

        var amountOwedByOther = payer.ExpenseParticipants
            .Where(ep => ep.Expense != null && ep.Expense.PayerId == debtor.Id)
            .Sum(ep => ep.AmountOwed);

        var refundFromOther = payer.ExpenseAsDebtor
            .Where(er => er.PayerId == debtor.Id)
            .Sum(er => er.AmountRefund);

        return amountOwedToOther - refundToOther - amountOwedByOther + refundFromOther;
    }
}
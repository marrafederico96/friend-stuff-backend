using System.Text.RegularExpressions;
using FriendStuffBackend.Data;
using FriendStuffBackend.Domain.Entities;
using FriendStuffBackend.Features.ExpenseEvent.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FriendStuffBackend.Features.ExpenseEvent;

public partial class ExpenseService(FriendStuffDbContext context) : IExpenseService
{
    
    [GeneratedRegex(@"[^a-zA-Z0-9]", RegexOptions.Compiled)]
    private static partial Regex InvalidChars();
    
    [GeneratedRegex(@"-+", RegexOptions.Compiled)]
    private static partial Regex MultipleDashes();
    
    public async Task AddExpense(ExpenseEventDto expenseData)
    {
        var normalizedEmail = expenseData.PayerUsername.Trim().ToLowerInvariant();
        var normalizedEventName = MultipleDashes()
            .Replace(
                InvalidChars().Replace(expenseData.EventName.TrimEnd().TrimStart(), "-"),
                "-"
            )
            .Trim('-').ToLowerInvariant();

        var payer = await context.Users
            .Where(u => u.Email == normalizedEmail)
            .Include(p => p.Events)
            .ThenInclude(e => e.Event)
            .FirstOrDefaultAsync() ?? throw new ArgumentException("Payer not found");

        var eventFound = payer.Events.FirstOrDefault(e => e.Event != null && e.Event.NormalizedEventName == normalizedEventName);
        if (eventFound == null)
        {
            throw new ArgumentException("Event not found");
        }

        Expense newExpense = new()
        {
            ExpenseDate = DateTime.UtcNow,
            Amount = Math.Round(expenseData.Amount,2),
            EventId = eventFound.EventId,
            ExpenseName = expenseData.ExpenseName.TrimEnd().TrimStart().ToLowerInvariant(),
            PayerId = payer.Id,
        };

        if (expenseData.ExpenseParticipant.Count == 0)
        {
            throw new ArgumentException("Participants required");
        }
        
        if ( expenseData.ExpenseParticipant.Count != 0)
        {
            var participantUsername = expenseData.ExpenseParticipant.Select(p => p?.UserName).ToList();
            var participants = await context.Users.Where(u => participantUsername.Contains(u.UserName))
                .ToListAsync();

            foreach (var participant in participants)
            {
                newExpense.Participants.Add(new ExpenseParticipant
                {
                    ParticipantId = participant.Id,
                    ExpenseId = newExpense.Id,
                    AmountOwed = Math.Round(newExpense.Amount / participants.Count, 2)
                });
            }
        }
        await context.Expenses.AddAsync(newExpense);
        var expensePayer = newExpense.Payer?.ExpenseParticipants
                               .Where(ex => ex.Expense != null && ex.Expense.ExpenseName == newExpense.ExpenseName)
                               .FirstOrDefault(ex => ex.Expense != null && ex.Expense.PayerId == payer.Id)
                           ?? throw new ArgumentException("Payer expense not found");

        context.ExpenseParticipants.Remove(expensePayer);
        await context.SaveChangesAsync();
    }

    public async Task<List<ResponseBalanceDto>> GetBalance(ExpenseBalanceDto balanceData)
    {
        var normalizedUsername = balanceData.LoggedUsername.Trim().ToLowerInvariant();

        var loggedUsername = await context.Users
            .Where(u => u.NormalizedUserName == normalizedUsername)
            .Include(u => u.ExpenseParticipants).ThenInclude(ex => ex.Expense)
            .Include(ex => ex.ExpenseAsDebtor)
            .FirstOrDefaultAsync() ?? throw new ArgumentException("Debtor not found");
        
        var otherUserIds = await context.Expenses
            .Where(e => e.PayerId == loggedUsername.Id || e.Participants.Any(p => p.ParticipantId == loggedUsername.Id))
            .SelectMany(e => e.Participants.Select(p => p.ParticipantId)
                .Union(new[] { e.PayerId }))
            .Where(uid => uid != loggedUsername.Id)
            .Distinct()
            .ToListAsync();
        
        var otherUsers = await context.Users
            .Where(u => otherUserIds.Contains(u.Id))
            .Include(u => u.ExpenseParticipants).ThenInclude(ep => ep.Expense)
            .Include(u => u.ExpenseAsDebtor)
            .ToListAsync();

        return (from other in otherUsers
            let amountOwedToOther = loggedUsername.ExpenseParticipants.Where(ep => ep.Expense != null && ep.Expense.PayerId == other.Id)
                .Sum(ep => ep.AmountOwed)
            let refundToOther = loggedUsername.ExpenseAsDebtor.Where(er => er.PayerId == other.Id)
                .Sum(er => er.AmountRefund)
            let amountOwedByOther = other.ExpenseParticipants.Where(ep => ep.Expense != null && ep.Expense.PayerId == loggedUsername.Id)
                .Sum(ep => ep.AmountOwed)
            let refundFromOther = other.ExpenseAsDebtor.Where(er => er.PayerId == loggedUsername.Id)
                .Sum(er => er.AmountRefund)
            let balanceAmount = amountOwedToOther - refundToOther - amountOwedByOther + refundFromOther
            select new ResponseBalanceDto { DebtorUsername = loggedUsername.UserName, PayerUsername = other.UserName, BalanceAmount = balanceAmount }).ToList();
    }
}
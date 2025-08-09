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
}
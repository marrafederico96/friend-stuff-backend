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
            Amount = expenseData.Amount,
            EventId = eventFound.EventId,
            ExpenseName = expenseData.ExpenseName,
            PayerId = payer.Id,
        };

        await context.Expenses.AddAsync(newExpense);
        await context.SaveChangesAsync();
    }
}
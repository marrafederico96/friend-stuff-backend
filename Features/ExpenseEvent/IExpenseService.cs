using FriendStuffBackend.Features.ExpenseEvent.DTOs;

namespace FriendStuffBackend.Features.ExpenseEvent;

public interface IExpenseService
{

    public Task AddExpense(ExpenseEventDto expenseData);

}
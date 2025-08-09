using FriendStuffBackend.Features.ExpenseEvent.DTOs;

namespace FriendStuffBackend.Features.ExpenseEvent
{
    /// <summary>
    /// Service responsible for managing expenses within events.
    /// </summary>
    public interface IExpenseService
    {
        /// <summary>
        /// Adds a new expense to an existing event.
        /// </summary>
        /// <param name="expenseData">
        /// DTO containing the expense details.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task AddExpense(ExpenseEventDto expenseData);
    }
}
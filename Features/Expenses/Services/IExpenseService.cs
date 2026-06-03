using FriendStuff.Domain.View;
using FriendStuff.Features.Expenses.DTOs;
using FriendStuff.Shared.Results;

namespace FriendStuff.Features.Expenses.Services;

public interface IExpenseService
{
    public Task<Result> CreateExpense(CreateExpenseRequest request, string payerUsername, CancellationToken ct);
    public Task<Result> AddExpenseParticipant(AddExpenseParticipantRequest request, CancellationToken ct);
    public Task<Result<List<ExpenseTypesResponse>>> GetExpenseTypes();


}

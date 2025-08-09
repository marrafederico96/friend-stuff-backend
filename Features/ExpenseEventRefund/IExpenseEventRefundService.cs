using FriendStuffBackend.Features.ExpenseEventRefund.DTOs;

namespace FriendStuffBackend.Features.ExpenseEventRefund;

public interface IExpenseEventRefundService
{
    public Task AddRefund(ExpenseEventRefundDto refundData);
}
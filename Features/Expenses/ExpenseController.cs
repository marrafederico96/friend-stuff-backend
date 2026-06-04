using FriendStuff.Extensions;
using FriendStuff.Features.Expenses.DTOs;
using FriendStuff.Features.Expenses.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendStuff.Features.Expenses
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ExpenseController(IExpenseService expenseService) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExpenseRequest request, CancellationToken ct)
        {
            var payerUsername = User.Identity?.Name ?? throw new ArgumentException("JWT not valid");

            var result = await expenseService.CreateExpense(request, payerUsername, ct);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> AddParticipants([FromBody] AddExpenseParticipantRequest request, CancellationToken ct)
        {
            var result = await expenseService.AddExpenseParticipant(request, ct);
            return result.ToActionResult();
        }

    }
}

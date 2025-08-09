using FriendStuffBackend.Features.ExpenseEvent.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FriendStuffBackend.Features.ExpenseEvent;

[ApiController]
[Route("/api/[controller]/[action]")]
public class ExpenseController(IExpenseService expenseService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] ExpenseEventDto expenseData)
    {
        try
        {
            await expenseService.AddExpense(expenseData);
            return Ok(new { message = "Expense added" });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new {message ="Internal Error."});
        }
    }
}
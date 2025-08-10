using FriendStuffBackend.Features.ExpenseEvent.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendStuffBackend.Features.ExpenseEvent;

[ApiController]
[Route("/api/[controller]/[action]")]
public class ExpenseController(IExpenseService expenseService) : ControllerBase
{
    [HttpPost]
    [Authorize]
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
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Balance([FromBody] ExpenseBalanceDto balanceData)
    {
        try
        {
            var balance = await expenseService.GetBalance(balanceData);
            return Ok(balance);
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
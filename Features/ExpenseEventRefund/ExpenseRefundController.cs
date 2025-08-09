using FriendStuffBackend.Features.ExpenseEventRefund.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendStuffBackend.Features.ExpenseEventRefund;

[Route("/api/[controller]/[action]")]
[ApiController]
public class ExpenseRefundController(IExpenseEventRefundService expenseEventRefundService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Add([FromBody] ExpenseEventRefundDto refundData)
    {
        try
        {
            await expenseEventRefundService.AddRefund(refundData);
            return Ok(new { message = "Refund added" });
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
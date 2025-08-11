using FriendStuffBackend.Features.EventMessage.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendStuffBackend.Features.EventMessage;

[Route("/api/[controller]/[action]")]
[ApiController]
public class MessageController(IEventMessageService messageService) : ControllerBase
{

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Send([FromBody] EventMessageDto messageData)
    {
        try
        {
            await messageService.SendMessage(messageData);
            return Ok(new { message = "Message sent" });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal error" });
        }
    }
    
}
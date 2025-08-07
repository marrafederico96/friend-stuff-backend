using FriendStuffBackend.Features.Account.DTOs;
using FriendStuffBackend.Features.UserEvent.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendStuffBackend.Features.UserEvent;

[ApiController]
[Route("api/[controller]/[action]")]
public class EventController(IEventService eventService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] EventDto eventData)
    {
        try
        {
            await eventService.CreateEvent(eventData);
            return Ok(new { message = "Event created" });
        }
        catch (ArgumentException e)
        {
            return Unauthorized(new { message = e.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal Error." });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Search([FromBody] SearchUserDto username)
    {
        try
        {
            var user = await eventService.SearchUser(username);
             return Ok(new {username = user});
        }
        catch (ArgumentException e)
        {
            return Unauthorized(new { message = e.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal Error." });
        }
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Add([FromBody] AddMemberDto userToAdd)
    {
        try
        {
           await eventService.AddMember(userToAdd);
            return Ok(new {message = "Member added"});
        }
        catch (ArgumentException e)
        {
            return Unauthorized(new { message = e.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal Error." });
        }
    }
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> Remove([FromBody] AddMemberDto userToAdd)
    {
        try
        {
            await eventService.RemoveMember(userToAdd);
            return Ok(new {message = "Member removed"});
        }
        catch (ArgumentException e)
        {
            return Unauthorized(new { message = e.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal Error." });
        }
    }
    

}
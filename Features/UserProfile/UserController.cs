using FriendStuff.Extensions;
using FriendStuff.Features.UserProfile.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendStuff.Features.UserProfile;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Balance(CancellationToken ct)
    {
        var username = User.FindFirst("name")?.Value ?? throw new ArgumentException("JWT not valid");
        var response = await userService.GenerateUserBalance(username, ct);

        return response.ToActionResult();
    }

}
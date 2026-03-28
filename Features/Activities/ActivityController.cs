using FriendStuff.Extensions;
using FriendStuff.Features.Activities.DTOs;
using FriendStuff.Features.Activities.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendStuff.Features.Activities
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ActivityController(IActivityService activityService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateActivityRequest request, CancellationToken ct)
        {
            var adminUsername = User.Identity?.Name ?? throw new ArgumentException("JWT not valid");

            var result = await activityService.CreateActivity(request, adminUsername, ct);
            return result.ToActionResult();
        }
    }
}

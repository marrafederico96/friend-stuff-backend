using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendStuff.Features.Activities
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ActivityController : ControllerBase
    {
    }
}

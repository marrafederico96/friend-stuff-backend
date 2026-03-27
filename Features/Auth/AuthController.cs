using FriendStuff.Extensions;
using FriendStuff.Features.Auth.DTOs;
using FriendStuff.Features.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FriendStuff.Features.Auth
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var response = await authService.AuthRegister(request, ct);
            return response.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var response = await authService.AuthLogin(request, ct);

            if (response.IsSuccess)
                Response.Cookies.Append("refresh_token", response.Value.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(14),
                    Path = "api/Auth/Refresh"
                });

            return response.ToActionResult();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var refreshTokenValue = Request.Cookies["refresh_token"] ?? throw new ArgumentException("Cookie not found");

            var result = await authService.AuthLogout(refreshTokenValue, ct);

            Response.Cookies.Delete("refresh_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
                Path = "api/Auth/Refresh"
            });

            return result.ToActionResult();
        }

    }
}

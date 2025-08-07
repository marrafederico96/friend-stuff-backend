using FriendStuffBackend.Features.Auth.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FriendStuffBackend.Features.Auth;

[ApiController]
[Route("/api/[controller]/[action]")]
public class AuthController(IAccountService accountService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerData)
    {
        try
        {
            await accountService.RegisterUser(registerData);
            return Ok(new {message = "User register."});
        }
        catch (ArgumentException e)
        {
            return Unauthorized(new {message = e.Message});
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new {message ="Internal Error."});
        }
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDto loginData)
    {
        try
        {
            var token = await accountService.LoginUser(loginData);
            
            HttpContext.Response.Cookies.Append("refresh_token", token.RefreshToken.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/api/auth/refresh",
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(15)
            });
            return Ok(new {accessToken = token.AccessToken});
        }
        catch (ArgumentException e)
        {
            return Unauthorized(new {message = e.Message});
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new {message ="Internal Error."});
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout([FromBody] UserInfoDto userInfoData)
    {
        try
        {
            await accountService.LogoutUser(userInfoData);
            HttpContext.Response.Cookies.Delete("refresh_token", new CookieOptions
            {
                Path = "/api/auth/refresh"
            });
            return Ok();
        }
        catch (ArgumentException e)
        {
            return Unauthorized(new {message = e.Message});
        } catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new {message ="Internal Error."});
        }
    }
}
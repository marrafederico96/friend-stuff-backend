using System.Security.Claims;
using FriendStuffBackend.Features.Account.DTOs;
using FriendStuffBackend.Features.Account.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendStuffBackend.Features.Account;

[ApiController]
[Route("/api/[controller]/[action]")]
public class AccountController(IAccountService accountService, ITokenService tokenService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerData) {
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
    public async Task<IActionResult> Login([FromBody] LoginDto loginData) {
        try
        {
            var token = await accountService.LoginUser(loginData);
            
            HttpContext.Response.Cookies.Append("refresh_token", token.RefreshToken.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/api/account/refresh",
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(15)
            });
            return Ok(new {access_token = token.AccessToken});
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
    public async Task<IActionResult> Refresh() {
        try
        {
            var refreshToken = HttpContext.Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { message = "Refresh token missing." });
            }
            var email = await tokenService.GetEmailByRefreshToken(refreshToken);
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(new { message = "Invalid refresh token." });
            }
            
            var isValid = await tokenService.CheckRefreshToken(refreshToken);
            if (!isValid)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token." });
            }
            
            HttpContext.Response.Cookies.Delete("refresh_token", new CookieOptions
            {
                Path = "/api/account/refresh"
            });
            var token = await tokenService.GenerateToken(email);
            HttpContext.Response.Cookies.Append("refresh_token", token.RefreshToken.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/api/account/refresh",
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(15),
            });
            return Ok(new { access_token = token.AccessToken });
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
    public async Task<IActionResult> Logout() {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(new { message = "Email claim missing." });
            }
            await accountService.LogoutUser(email);
            HttpContext.Response.Cookies.Delete("refresh_token", new CookieOptions
            {
                Path = "/api/account/refresh"
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

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUser() {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(new { message = "Email claim missing." });
            }

            var userInfo = await accountService.GetUserInfo(email);
            return Ok(userInfo);
        }
        catch (ArgumentException e)
        {
            return Unauthorized(new {message = e.Message});
        } catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new {message ="Internal Error."});
        }
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> Delete()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(new { message = "Email claim missing." });
            }

            await accountService.DeleteUser(email);
            return Ok(new {message="User delete"});
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
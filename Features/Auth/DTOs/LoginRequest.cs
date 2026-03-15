using System.ComponentModel.DataAnnotations;

namespace FriendStuff.Features.Auth.DTOs;

public record class LoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string EmailAddress { get; set; } = string.Empty;


    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}

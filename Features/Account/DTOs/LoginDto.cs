using System.ComponentModel.DataAnnotations;

namespace FriendStuffBackend.Features.Auth.DTOs;

public record LoginDto
{
    [Required(ErrorMessage = "Email cannot be empty.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public required string Email { get; init; }

    [Required(ErrorMessage = "Password cannot be empty.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public required string Password { get; init; }
}
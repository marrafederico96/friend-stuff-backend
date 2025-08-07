using System.ComponentModel.DataAnnotations;

namespace FriendStuffBackend.Features.Auth.Token.DTOs;

public record TokenDto
{
    [Required]
    public required string AccessToken;

    [Required] 
    public required Guid RefreshToken;
};
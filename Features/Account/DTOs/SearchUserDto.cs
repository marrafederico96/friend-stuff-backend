using System.ComponentModel.DataAnnotations;

namespace FriendStuffBackend.Features.Account.DTOs;

public record SearchUserDto
{
    [Required(ErrorMessage = "Username required for logout")]
    public required string UserName { get; init; }

}
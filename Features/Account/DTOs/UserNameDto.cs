using System.ComponentModel.DataAnnotations;

namespace FriendStuffBackend.Features.Account.DTOs;

public record UserNameDto
{
    [Required(ErrorMessage = "Username required")]
    public required string UserName { get; init; }

}
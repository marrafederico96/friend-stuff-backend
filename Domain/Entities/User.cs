using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FriendStuff.Domain.Entities;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Username { get; set; }


    [Required]
    [MaxLength(100)]
    public required string NormalizedUsername { get; set; }

    [Required]
    [MaxLength(100)]
    public required string EmailAddress { get; set; }

    [Required]
    [MaxLength(100)]
    public required string NormalizedEmailAddress { get; set; }

    [Required]
    [MaxLength(600)]
    public required string PasswordHash { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    private User() { }

    public User Create(string username, string emailAddress, string password, PasswordHasher<User> passwordHasher)
    {
        var newUser = new User
        {
            Username = username,
            EmailAddress = emailAddress,
            NormalizedUsername = username.Trim().ToUpperInvariant(),
            NormalizedEmailAddress = emailAddress.Trim().ToUpperInvariant(),
            PasswordHash = ""
        };
        newUser.PasswordHash = passwordHasher.HashPassword(newUser, password);
        return newUser;
    }
}

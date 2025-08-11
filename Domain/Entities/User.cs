using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FriendStuffBackend.Features.ExpenseEventRefund.DTOs;

namespace FriendStuffBackend.Domain.Entities;

[Table("users")]
public class User
{
    [Key]
    public Guid Id { get; init; }
    
    [Column("username", TypeName = "text")]
    [MaxLength(100)]
    public required string UserName { get; init; }
    
    [Column("email", TypeName = "text")]
    [MaxLength(100)]
    public required string Email { get; init; }
    
    [Column("normalized_username", TypeName = "text")]
    [MaxLength(100)]
    public required string NormalizedUserName { get; init; }

    [Column("password_hash", TypeName = "text")]
    [MaxLength(256)]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Column("created_at")]
    public required DateTime CreatedAt { get; init; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<EventUser> Events { get; set; } = [];
    public ICollection<ExpenseParticipant> ExpenseParticipants { get; set; } = [];
    public ICollection<ExpenseRefund> ExpenseAsDebtor { get; set; } = [];
    public ICollection<ExpenseRefund> ExpenseAsPayer { get; set; } = [];
    public ICollection<Expense> ExpensesPayed { get; init; } = [];

}
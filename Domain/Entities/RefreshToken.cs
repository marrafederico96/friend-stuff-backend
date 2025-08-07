using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FriendStuffBackend.Domain.Entities;

[Table("refresh_tokens")]
public class RefreshToken
{
    [Key]
    public Guid Id { get; init; }
    
    [Column("created_at")]
    public required DateTime CreatedAt { get; init; }
    
    [Column("token_value")]
    public required Guid TokenValue { get; init; }
    
    [Column("expire_at")]
    public required DateTime ExpireAt { get; init; }
    
    [Column("is_valid")]
    public required bool IsValid { get; set; }
    
    [Column("user_id")]
    public required Guid UserId { get; init; }
    
    [ForeignKey("UserId")]
    public User? User { get; init; }
    
}
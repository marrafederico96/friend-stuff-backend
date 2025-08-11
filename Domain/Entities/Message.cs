using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FriendStuffBackend.Domain.Entities;

[Table("messages")]
public class Message
{
    [Key]
    public Guid Id { get; init; }
    
    [Column("send_date")]
    public DateTime SendDate { get; init; }
    
    [Column("sender_id")]
    public Guid SenderId { get; set; }
    
    [Column("content", TypeName = "text")] 
    public required string Content { get; set; }
    
    [Column("event_id")]
    public Guid EventId { get; set; }
    
    [ForeignKey("EventId")]
    public Event? Event { get; set; }
    
    [ForeignKey("SenderId")]
    public User? Sender { get; init; }
    
}
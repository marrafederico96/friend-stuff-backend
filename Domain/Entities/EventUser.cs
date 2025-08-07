using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FriendStuffBackend.Domain.Entities.Enum;

namespace FriendStuffBackend.Domain.Entities;

[Table("event_user")]
public class EventUser
{
    [Key] 
    public Guid EventUserId { get; set; }
    
    [Column("event_id")]
    public Guid EventId { get; set; }
    
    [Column("participant_id")]
    public Guid ParticipantId { get; set; }
    
    [Column("user_role")]
    public EventUserRole UserRole { get; set; }

    [ForeignKey("EventId")] 
    public Event? Event { get; set; }

    [ForeignKey("ParticipantId")] 
    public User? Participant { get; set; }
}
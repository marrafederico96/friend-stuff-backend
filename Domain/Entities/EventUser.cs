using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FriendStuffBackend.Domain.Entities.Enum;

namespace FriendStuffBackend.Domain.Entities;

[Table("event_user")]
public class EventUser
{
    [Key] 
    public Guid EventUserId { get; init; }
    
    [Column("event_id")]
    public Guid EventId { get; init; }
    
    [Column("participant_id")]
    public Guid ParticipantId { get; init; }
    
    [Column("user_role")]
    public EventUserRole UserRole { get; init; }

    [ForeignKey("EventId")] 
    public Event Event { get; init; }

    [ForeignKey("ParticipantId")] 
    public User Participant { get; init; }
}
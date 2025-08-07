using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FriendStuffBackend.Domain.Entities;

[Table("events")]
public class Event
{
    [Key]
    public Guid Id { get; init; }
    
    [Column("event_name", TypeName = "text")]
    [MaxLength(100)]
    public required string EventName { get; set; }

    [Column("normalized_event_name", TypeName = "text")]
    [MaxLength(100)]
    public required string NormalizedEventName { get; set; }
    
    [Column("start_date")]
    public DateOnly StartDate { get; set; }
    
    [Column("end_date")]
    public DateOnly EndDate { get; set; }
    
    [Column("admin_id")]
    public Guid AdminId { get; set; }

    [ForeignKey("AdminId")] 
    public User? Admin { get; set; }
    
    public ICollection<EventUser> Participants { get; set; } = [];
    
}
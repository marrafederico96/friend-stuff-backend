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
    public required string EventName { get; init; }

    [Column("normalized_event_name", TypeName = "text")]
    [MaxLength(100)]
    public required string NormalizedEventName { get; init; }
    
    [Column("start_date")]
    public DateOnly StartDate { get; init; }
    
    [Column("end_date")]
    public DateOnly EndDate { get; init; }
    
    [Column("admin_id")]
    public Guid AdminId { get; init; }

    [ForeignKey("AdminId")] 
    public User? Admin { get; init; }
    
    public ICollection<EventUser> Participants { get; init; } = [];

    public ICollection<Expense> Expenses { get; init; } = [];

    public ICollection<Message> Messages { get; set; } = [];

}
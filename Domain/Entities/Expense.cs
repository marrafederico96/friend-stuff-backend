using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FriendStuffBackend.Domain.Entities;

[Table("expenses")]
public class Expense
{
    [Key]
    public Guid Id { get; init; }

    [Column("event_id")]
    public required Guid EventId { get; init; }
    
    [Column("payer_id")]
    public required Guid PayerId { get; init; }
    
    [Column("expense_name", TypeName = "text")]
    [MaxLength(100)]
    public required string ExpenseName { get; init; }
    
    [Column("amount")]
    public required decimal Amount { get; init; }
    
    [Column("expense_date")]
    public required DateTime ExpenseDate { get; init; }
    
    [ForeignKey("EventId")]
    public Event? Event { get; init; }

    [ForeignKey("PayerId")]
    public User? Payer { get; init; }

    public ICollection<ExpenseParticipant> Participants { get; init; } = [];

}
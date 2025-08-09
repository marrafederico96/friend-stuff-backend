using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FriendStuffBackend.Domain.Entities;

[Table("expense_participants")]
public class ExpenseParticipant
{
    [Key]
    public Guid Id { get; init; }
    
    [Column("participant_id")]
    public required Guid ParticipantId { get; init; }
    
    [Column("expense_id")]
    public required Guid ExpenseId { get; init; }
    
    [ForeignKey("ParticipantId")]
    public User? Participant { get; init; }
    
    [ForeignKey(("ExpenseId"))]
    public Expense? Expense { get; init; }
    
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FriendStuffBackend.Domain.Entities;

[Table("expense_participants")]
public class ExpenseParticipant
{
    [Key]
    public Guid Id { get; set; }
    
    [Column("participant_id")]
    public required Guid ParticipantId { get; set; }
    
    [Column("expense_id")]
    public required Guid ExpenseId { get; set; }
    
    [ForeignKey("ParticipantId")]
    public User? Participant { get; set; }
    
    [ForeignKey(("ExpenseId"))]
    public Expense? Expense { get; set; }
    
}
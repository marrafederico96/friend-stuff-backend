using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FriendStuffBackend.Domain.Entities;

[Table(("expense_refund"))]
public class ExpenseRefund
{
    [Key]
    public Guid Id { get; init; }
    
    [Column("refund_date")]
    public DateTime RefundDate { get; init; }
    
    [Column("debtor_id")]
    public Guid DebtorId { get; init; }
    
    [Column("payer_id")]
    public Guid PayerId { get; init; }
    
    [Column("amount_refund")]
    public decimal AmountRefund { get; init; }
    
    [ForeignKey("DebtorId")]
    public User? Debtor { get; init; }
    
    [ForeignKey("PayerId")]
    public User? Payer { get; init; }
    
}
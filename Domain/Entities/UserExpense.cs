using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FriendStuff.Domain.Entities;

public class UserExpense
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required int ExpenseId { get; set; }

    [Required]
    public required int DebtorId { get; set; }

    [Required]
    public required decimal AmountOwed { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(DebtorId))]
    public User? Debtor { get; set; }

    [ForeignKey(nameof(ExpenseId))]
    public Expense? Expense { get; set; }

}

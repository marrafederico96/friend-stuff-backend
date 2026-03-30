using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FriendStuff.Domain.Enums;

namespace FriendStuff.Domain.Entities;

public class Expense
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required int PayerId { get; set; }

    [Required]
    public required int ActivityId { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(256)]
    public string? Description { get; set; }

    [Required]
    public required decimal Amount { get; set; }

    [Required]
    public required ExpenseType Type { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public List<UserExpense> Participants { get; set; } = [];

    [ForeignKey(nameof(PayerId))]
    public User? Payer { get; set; }

    [ForeignKey(nameof(ActivityId))]
    public Activity? Activity { get; set; }

}

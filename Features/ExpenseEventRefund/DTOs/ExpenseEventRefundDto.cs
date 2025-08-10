using System.ComponentModel.DataAnnotations;

namespace FriendStuffBackend.Features.ExpenseEventRefund.DTOs;

public record ExpenseEventRefundDto
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Refund must be greater than zero")]
    public required decimal AmountRefund { get; set; }
    
    [Required]
    public required string PayerUsername { get; init; } 
    
    [Required]
    public required string DebtorUsername { get; init; }
    
}
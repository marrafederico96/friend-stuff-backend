using System.ComponentModel.DataAnnotations;

namespace FriendStuff.Features.Expenses.DTOs
{
    public class AddExpenseParticipantRequest
    {
        [Required]
        public List<string> Usernames {get;set;}= [];

        [Required]
        public string PublicActivityId { get; set; } = string.Empty;

        [Required]
        public string PublicExpenseId { get; set; } = string.Empty;
    }
}

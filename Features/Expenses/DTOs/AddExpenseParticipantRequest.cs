namespace FriendStuff.Features.Expenses.DTOs
{
    public class AddExpenseParticipantRequest
    {
        public List<string> Usernames = [];

        public string PublicActivityId = string.Empty;
        
        public string PublicExpenseId = string.Empty;
    }
}

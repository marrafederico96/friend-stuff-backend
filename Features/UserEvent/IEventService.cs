using FriendStuffBackend.Features.Account.DTOs;
using FriendStuffBackend.Features.UserEvent.DTOs;

namespace FriendStuffBackend.Features.UserEvent
{
    /// <summary>
    /// Defines the contract for managing user events.
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Asynchronously creates a new event using the provided event data.
        /// </summary>
        /// <param name="eventData">An <see cref="EventDto"/> containing the details of the event to be created.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CreateEvent(EventDto eventData);

        /// <summary>
        /// Searches for a user by username.
        /// </summary>
        /// <param name="username">A <see cref="UserNameDto"/> containing the username to search for.</param>
        /// <returns>
        /// A <see cref="Task{String}"/> representing the asynchronous operation, 
        /// with a string result containing the search outcome or user identifier.
        /// </returns>
        Task<string> SearchUser(UserNameDto username);

        /// <summary>
        /// Adds a member to an event.
        /// </summary>
        /// <param name="userToEvent">An <see cref="EventMemberDto"/> containing information about the user to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddMember(EventMemberDto userToEvent);

        /// <summary>
        /// Removes a member from an event.
        /// </summary>
        /// <param name="userToRemove">An <see cref="EventMemberDto"/> containing information about the user to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveMember(EventMemberDto userToRemove);
    }
}
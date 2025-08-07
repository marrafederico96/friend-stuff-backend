using FriendStuffBackend.Features.Account.DTOs;
using FriendStuffBackend.Features.UserEvent.DTOs;

namespace FriendStuffBackend.Features.UserEvent;

/// <summary>
/// Defines the contract for managing user events.
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Asynchronously creates a new event using the provided event data.
    /// </summary>
    /// <param name="eventData">
    /// An <see cref="EventDto"/> containing the details of the event to be created.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task CreateEvent(EventDto eventData);


    public Task<string> SearchUser(SearchUserDto username);

    public  Task AddMember(AddMemberDto userToAdd);
    
    public  Task RemoveMember(AddMemberDto userToRemove);



}
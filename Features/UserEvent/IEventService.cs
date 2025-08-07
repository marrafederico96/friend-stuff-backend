using FriendStuffBackend.Features.Event.DTOs;

namespace FriendStuffBackend.Features.Event;

public interface IEventService
{
    public Task CreateEvent(EventDto eventData);
}
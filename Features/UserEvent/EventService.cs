using FriendStuffBackend.Data;
using FriendStuffBackend.Features.Event.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FriendStuffBackend.Features.Event;

public class EventService(FriendStuffDbContext context) : IEventService
{
    public async Task CreateEvent(EventDto eventData)
    {
        var normalizedEmail = eventData.AdminEmail.Trim().ToLowerInvariant();
        var normalizedEventName = eventData.EventName.TrimStart().TrimEnd().ToLowerInvariant();
        var eventExists = await context.Events
            .AnyAsync(e => e.Admin.Email == normalizedEmail && e.EventName == normalizedEventName );
        if (eventExists)
        {
            throw new ArgumentException("Event already exists");
        }
    }
}
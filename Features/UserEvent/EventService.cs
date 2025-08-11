using System.Text.RegularExpressions;
using FriendStuffBackend.Data;
using FriendStuffBackend.Domain.Entities;
using FriendStuffBackend.Domain.Entities.Enum;
using FriendStuffBackend.Features.Account.DTOs;
using FriendStuffBackend.Features.UserEvent.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FriendStuffBackend.Features.UserEvent;

public partial class EventService(FriendStuffDbContext context) : IEventService
{

    [GeneratedRegex(@"[^a-zA-Z0-9]", RegexOptions.Compiled)]
    private static partial Regex InvalidChars();
    
    [GeneratedRegex(@"-+", RegexOptions.Compiled)]
    private static partial Regex MultipleDashes();
    
    public async Task CreateEvent(EventDto eventData)
    {
        var normalizedEventName = MultipleDashes()
            .Replace(
                InvalidChars().Replace(eventData.EventName.TrimEnd().TrimStart(), "-"),
                "-"
            )
            .Trim('-').ToLowerInvariant();
        var normalizedEmail = eventData.AdminEmail.Trim().ToLowerInvariant();

        if (eventData.EndDate < eventData.StartDate)
        {
            throw new ArgumentException("End date cannot be earlier than start date.");
        }
        
        var admin = await context.Users
            .Where(u => u.Email == normalizedEmail)
            .FirstOrDefaultAsync() ?? throw new ArgumentException("Admin not found.");
        
        var eventExists = await context.Events
            .AnyAsync(e => e.Admin != null && e.Admin.Email == normalizedEmail && e.NormalizedEventName == normalizedEventName );
        if (eventExists)
        {
            throw new ArgumentException("User event already exists.");
        }

        Event newEvent = new()
        {
            EventName = eventData.EventName.TrimStart().TrimEnd(),
            NormalizedEventName = normalizedEventName,
            AdminId = admin.Id,
            EndDate = eventData.EndDate,
            StartDate = eventData.StartDate,
        };
        newEvent.Participants.Add(new EventUser
        {
            EventId = newEvent.Id,
            ParticipantId = admin.Id,
            UserRole = EventUserRole.Admin,
            Event = newEvent,
        });

        await context.Events.AddAsync(newEvent);
        await context.SaveChangesAsync();
    }

    public async Task<string> SearchUser(UserNameDto userName)
    {
        var normalizedUsername = userName.UserName.Trim().ToLowerInvariant();
        var userFound = await context.Users.Where(u => u.NormalizedUserName == normalizedUsername).FirstOrDefaultAsync() ?? throw new ArgumentException("User not found");

        return userFound.UserName;
    }

    public async Task AddMember(EventMemberDto userToEvent)
    {
        var normalizedUsername = userToEvent.UserName.Trim().ToLowerInvariant();
        var normalizedAdminUsername = userToEvent.AdminUsername.Trim().ToLowerInvariant();
        
        var userFound = await context.Users
            .Where(u => u.NormalizedUserName == normalizedUsername)
            .Include(u => u.Events)
            .ThenInclude(e => e.Event)
            .FirstOrDefaultAsync() ?? throw new ArgumentException("User not found");

        var result = userFound.Events.Any(e => e.Event != null && e.Event.NormalizedEventName == userToEvent.NormalizedEventName);
        if (result)
        {
            throw new ArgumentException("User already added");
        }
        var eventFound = await
            context.Events
                .FirstOrDefaultAsync(e => e.NormalizedEventName == userToEvent.NormalizedEventName && e.Admin != null && e.Admin.NormalizedUserName == normalizedAdminUsername) ?? throw new ArgumentException("Event not found");


        var newParticipant = new EventUser
        {
            EventId = eventFound.Id,
            ParticipantId = userFound.Id,
            UserRole = EventUserRole.Member,
        };

        await context.EventUsers.AddAsync(newParticipant);
        await context.SaveChangesAsync();
    }

    public async Task RemoveMember(EventMemberDto userToRemove)
    {
        var normalizedUsername = userToRemove.UserName.Trim().ToLowerInvariant();
        
        var userFound = await context.Users
            .Where(u => u.NormalizedUserName == normalizedUsername)
            .Include(u => u.Events)
            .ThenInclude(e => e.Event)
            .FirstOrDefaultAsync() ?? throw new ArgumentException("User not found");
        
        var result = userFound.Events.Any(e => e.Event != null && e.Event.NormalizedEventName == userToRemove.NormalizedEventName);
        if (!result)
        {
            throw new ArgumentException("Event not found");
        }

        var eventUser = await context.EventUsers.Where(u => u.ParticipantId == userFound.Id).FirstOrDefaultAsync() ?? throw new ArgumentException("User not found");

        context.EventUsers.Remove(eventUser);
        await context.SaveChangesAsync();
    }
}
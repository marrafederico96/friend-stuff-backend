using FriendStuffBackend.Data;
using FriendStuffBackend.Domain.Entities;
using FriendStuffBackend.Features.EventMessage.DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FriendStuffBackend.Features.EventMessage;

public class EventMessageService(FriendStuffDbContext context, IHubContext<MessageHub> hubContext) : IEventMessageService
{
    public async Task SendMessage(EventMessageDto messageData)
    {
        var normalizedSenderUsername = messageData.SenderUsername.Trim().ToLowerInvariant();
        var messageContent = messageData.MessageContent;
        var eventName = messageData.NormalizedEventName;
        
        var sender = await context.Users
            .Where(u => u.NormalizedUserName == normalizedSenderUsername)
            .Include(u => u.Events).ThenInclude(eventUser => eventUser.Event)
            .FirstOrDefaultAsync() ?? throw new ArgumentException("Sender not found");

        var eventUser = sender.Events
            .FirstOrDefault(e => e.Event != null && e.Event.NormalizedEventName == eventName);

        if (eventUser != null)
        {
            var newMessage = new Message
            {
                Content = messageContent,
                EventId = eventUser.EventId,
                SendDate = DateTime.UtcNow,
                SenderId = sender.Id,
            };
            await context.Messages.AddAsync(newMessage);
            await context.SaveChangesAsync();

            await hubContext.Clients.Group(eventName)
                .SendAsync("ReceiveMessage", new
                {
                    messageContent= newMessage.Content,
                    SenderUsername = sender.UserName,
                    EventName = eventName
                });

        }
        else
        {
            throw new ArgumentException("Event not found");
        }
    }
}
using FriendStuffBackend.Features.EventMessage.DTOs;

namespace FriendStuffBackend.Features.EventMessage;

public interface IEventMessageService
{
    public Task SendMessage(EventMessageDto messageData);

}
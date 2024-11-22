using WebApplication1.Events;

namespace WebApplication1;

public interface IMessageBroker
{
    Task SendAsync(string @event);
}

public class InMemoryMessageBroker : IMessageBroker
{
    public Task SendAsync(string @event)
    {
        Console.WriteLine($"Sending event to messageBroker: {@event}");
        return Task.CompletedTask;
    }
}
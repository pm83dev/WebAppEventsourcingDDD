using WebApplication1.Events;

namespace WebApplication1;

public interface IEventPublisher
{
    Task PublishAsync(Event @event);
    Task PublishAllAsync(IEnumerable<Event> events);
}

public class EventPublisher : IEventPublisher
{
    private readonly IMessageBroker _messageBroker;

    public EventPublisher(IMessageBroker messageBroker)
    {
        _messageBroker = messageBroker ?? throw new ArgumentNullException(nameof(messageBroker));
    }

    public async Task PublishAsync(Event @event)
    {
        if (@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        try
        {
            Console.WriteLine($"Publishing event: {@event.GetType().Name}");
            await _messageBroker.SendAsync(@event);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error publishing event: {ex.Message}");
            throw; // Rilancia l'eccezione se necessario
        }
    }

    public async Task PublishAllAsync(IEnumerable<Event> events)
    {
        if (events == null)
        {
            throw new ArgumentNullException(nameof(events));
        }

        foreach (var @event in events)
        {
            await PublishAsync(@event);
        }
    }
}

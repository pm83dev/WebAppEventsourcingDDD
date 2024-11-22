using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
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
            // Configura il serializer per il polimorfismo
            var options = new JsonSerializerOptions
            {
                WriteIndented = true, // Per leggibilità
                TypeInfoResolver = new DefaultJsonTypeInfoResolver()
            };

            // Serializza l'evento e stampalo
            var eventDetails = JsonSerializer.Serialize(@event, options);
            Console.WriteLine($"Publishing event: {eventDetails}");

            // Invia l'evento tramite il message broker
            await _messageBroker.SendAsync(eventDetails);
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

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using WebApplication1.Events;
using RabbitMQ.Client;

namespace WebApplication1;

public interface IEventPublisher
{
    Task PublishAsync(Event @event);
    //Task PublishAllAsync(IEnumerable<Event> events);
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


public class RabbitMqEventPublisher : IEventPublisher, IDisposable
{
    private readonly string _hostname = "localhost";
    private readonly string _queueName = "eventQueue";
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMqEventPublisher()
    {
        var factory = new ConnectionFactory
        {
            HostName = _hostname,
            Port = 5672, // Porta di default RabbitMQ
            VirtualHost = "/",
        };

        // Inizializza connessione e canale
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        // Dichiara la coda
        _channel.QueueDeclareAsync(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null).GetAwaiter().GetResult();
    }

    public async Task PublishAsync(Event @event)
    {
        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: _queueName,
            body: body);

        Console.WriteLine($"Published event on RabbitMq: {message}");
    }

    public void Dispose()
    {
        _channel?.CloseAsync();
        _connection?.CloseAsync();
    }
}

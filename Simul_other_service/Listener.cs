using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WebApplication1.Events;

namespace WebApplication1;

public interface IEventListener
{
    Task ListenAsync();
    List<string> GetMessages(); 
}
   
public class RabbitMqEventListener  : IEventListener
{
    private readonly string _hostname = "localhost";
    private readonly string _queueName = "eventQueue"; 
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly List<string> _receivedMessages;

    public RabbitMqEventListener()
    {
        _receivedMessages = new List<string>();
        var factory = new ConnectionFactory
        {
            HostName = _hostname,
            Port = 5672,
            VirtualHost = "/"
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

    public async Task ListenAsync()
    {
       
        Console.WriteLine(" [*] Waiting for messages.");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var @event = JsonSerializer.Deserialize<Event>(message);

            if (@event != null)
            {
                Console.WriteLine($"Evento ricevuto: {@event.GetType().Name}");
                SaveMessages(message);   
                await HandleEventAsync(@event);
            }
        };

        _channel.BasicConsumeAsync(queue: _queueName,
            autoAck: true,
            consumer: consumer);

        Console.WriteLine("In ascolto degli eventi...");
        await Task.Delay(-1); // Mantieni in esecuzione per sempre
    }

    private Task HandleEventAsync(Event @event)
    {
        switch (@event)
        {
            case OrderCreated orderCreated:
                Console.WriteLine($"Ordine creato: {orderCreated.OrderId} - Qta:{orderCreated.OrderQty} - DT:{orderCreated.CreatedAt}");
                break;
            case OrderUpdated orderUpdated:
                Console.WriteLine($"Ordine aggiornato: {orderUpdated.OrderId} - Qta:{orderUpdated.OrderQty} - DT:{orderUpdated.CreatedAt}");
                break;
            default:
                Console.WriteLine("Evento sconosciuto.");
                break;
        }

        return Task.CompletedTask;
    }

    public void SaveMessages(string message)
    {
        _receivedMessages.Add(message);
        Console.WriteLine($"Messaggio aggiunto alla lista: {message}");
        // Stampa i messaggi separati da una virgola
        Console.WriteLine($"Messaggi Totali: {string.Join(", ", _receivedMessages)}");
    }

    // Metodo per ottenere la lista messaggi
    public List<string> GetMessages()
    {
        return _receivedMessages;
    }
}
    
    

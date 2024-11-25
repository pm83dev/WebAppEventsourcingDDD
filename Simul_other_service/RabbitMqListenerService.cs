namespace WebApplication1;

using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

public class RabbitMqListenerService : IHostedService
{
    private readonly IEventListener _eventListener;

    public RabbitMqListenerService(IEventListener eventListener)
    {
        _eventListener = eventListener;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Avvia ListenAsync in un task separato
        Task.Run(() => _eventListener.ListenAsync(), cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Qui puoi implementare la logica per fermare l'ascolto, se necessario
        return Task.CompletedTask;
    }
}

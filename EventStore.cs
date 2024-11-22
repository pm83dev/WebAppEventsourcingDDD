using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebApplication1.Events;

namespace WebApplication1
{
    public interface IEventStore
    {
        Task SaveEventStoreAsync(Event @event);
        Task<AggregateRoot> GetEventStoreAsync(Guid orderId);
        Task<IEnumerable<Event>> GetEventListAsync(Guid orderId);
    }

    public class InMemoryEventStore : IEventStore
    {
        //private readonly ConcurrentDictionary<Guid, List<Event>> ?_store = new();
        private readonly Dictionary<Guid, SortedList<DateTime, Event>> _store = new(); 

        public Task SaveEventStoreAsync(Event @event)
        {
            var stream = _store!.GetValueOrDefault(@event.StreamId, null);
            if (stream == null)
            {
                _store[@event.StreamId] = new SortedList<DateTime, Event>();
            }
    
            @event.CreatedAt = DateTime.Now;
    
            _store[@event.StreamId].Add(@event.CreatedAt, @event);
            return Task.CompletedTask;
        }


        
        public Task<AggregateRoot> GetEventStoreAsync(Guid orderId)
        {
            if (!_store.TryGetValue(orderId, out var orderEvents))
            {
                return Task.FromResult<AggregateRoot>(null);
            }
            var order = new AggregateRoot();
            foreach (var orderEvent in orderEvents)
            {
                order.Apply(orderEvent.Value); // Applica evento per evento
            }
            return Task.FromResult(order); // Restituisce l'aggregate root.
        }
        
        
        public Task<IEnumerable<Event>> GetEventListAsync(Guid orderId)
        {
            if (!_store.TryGetValue(orderId, out var orderEvents))
            {
                return Task.FromResult<IEnumerable<Event>>(Enumerable.Empty<Event>());
            }

            return Task.FromResult(orderEvents.Select(e => e.Value));
        }





    }

}

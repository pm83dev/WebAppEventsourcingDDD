using WebApplication1.Events;
using MongoDB.Driver;

namespace WebApplication1
{
    public interface IEventStore
    {
        Task SaveEventStoreAsync(Event @event);
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
        
        
        public Task<IEnumerable<Event>> GetEventListAsync(Guid orderId)
        {
            if (!_store.TryGetValue(orderId, out var orderEvents))
            {
                return Task.FromResult<IEnumerable<Event>>([]);
            }

            return Task.FromResult(orderEvents.Select(e => e.Value));
        }





    }

    public class MongoDbEventStore : IEventStore
    {
        private readonly IMongoCollection<Event> _mongoStore;
        public MongoDbEventStore(IMongoClient mongoClient)
        {
            if (mongoClient == null)
            {
                throw new ArgumentNullException(nameof(mongoClient));
            }

            var database = mongoClient.GetDatabase("OrderEventStore");
            _mongoStore = database.GetCollection<Event>("Events");
        }

        public async Task SaveEventStoreAsync(Event @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            
            @event.CreatedAt = DateTime.Now;
            try
            {
                await _mongoStore.InsertOneAsync(@event);
                Console.WriteLine($"Event saved to MongoDB, orderId: {@event.StreamId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il salvataggio in mongoDb: {ex.Message}");
            }

        }
        
        
        public async Task<IEnumerable<Event>> GetEventListAsync(Guid orderId)
        {
            try
            {
                // Usa "OrderId" come filtro, presente in tutte le sottoclassi
                var filter = Builders<Event>.Filter.Eq("OrderId", orderId);

                // Esegui la query
                var result = await _mongoStore.Find(filter).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        
    }

}

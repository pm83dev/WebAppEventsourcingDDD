using WebApplication1.Events;

namespace WebApplication1
{
    public class CommandHandler
    {
        private readonly IEventStore _eventStore;
        private readonly IEventPublisher _eventPublisher;

        public CommandHandler(IEventStore eventStore, IEventPublisher eventPublisher)
        {
            _eventStore = eventStore;
            _eventPublisher = eventPublisher;
        }

        public async Task HandleCreateOrderAsync(Guid orderId, Guid customerId, int orderQty)
        {
            // 1. Crea un nuovo aggregate o ricostruisci esistente
            var aggregateRoot = new AggregateRoot();

            // 2. Genera l'evento OrderCreated
            var orderCreatedEvent = new OrderCreated
            {
                OrderId = orderId,
                CustomerId = customerId,
                OrderQty = orderQty,
                CreatedAt = DateTime.UtcNow
            };

            // 3. Aggiungi evento non commesso all'aggregate
            aggregateRoot.AddUncommittedEvent(orderCreatedEvent);

            // 4. Applica l'evento all'aggregate
            aggregateRoot.Apply(orderCreatedEvent);

            // 5. Salva l'aggregate nell'Event Store
            foreach (var @event in aggregateRoot.UncommittedEvents)
            {
                await _eventStore.SaveEventStoreAsync(@event);  // Passa solo l'evento, non l'intero aggregate
            }

            // 6. Pubblica gli eventi non commessi
            foreach (var @event in aggregateRoot.UncommittedEvents)
            {
               await _eventPublisher.PublishAsync(@event);
            }

            // 7. Pulisci gli eventi non commessi
            aggregateRoot.ClearUncommittedEvents();
        }

        public async Task HandleUpdateOrderAsync(Guid orderId, Guid customerId, int orderQty)
        {
            // 1. Ricostruisci l'aggregate dall'Event Store
            var events = await _eventStore.GetEventListAsync(orderId);
            var aggregateRoot = new AggregateRoot();
            foreach (var @event in events)
            {
                aggregateRoot.Apply(@event);
            }

            // 2. Genera l'evento OrderUpdated
            var orderUpdatedEvent = new OrderUpdated
            {
                OrderId = orderId,
                CustomerId = customerId,
                OrderQty = orderQty,
                CreatedAt = DateTime.UtcNow
            };

            // 3. Aggiungi e applica l'evento
            aggregateRoot.AddUncommittedEvent(orderUpdatedEvent);
            aggregateRoot.Apply(orderUpdatedEvent);
            
            // 4. Salva l'aggregate nell'Event Store
            foreach (var @event in aggregateRoot.UncommittedEvents)
            {
                await _eventStore.SaveEventStoreAsync(@event);  // Passa solo l'evento, non l'intero aggregate
            }
            
            // 5. Pubblica gli eventi
            foreach (var @event in aggregateRoot.UncommittedEvents)
            {
                await _eventPublisher.PublishAsync(@event);
            }

            // 6. Pulisci gli eventi non commessi
            aggregateRoot.ClearUncommittedEvents();
        }

        public Task<IEnumerable<Event>> HandleGetOrderAsync(Guid orderId)
        {
            // Recupera la lista di eventi dall'Event Store
            return _eventStore.GetEventListAsync(orderId);
        }

        public async Task HandleDeleteOrderAsync(Guid orderId)
        {
            throw new NotImplementedException();
        }
    }
}

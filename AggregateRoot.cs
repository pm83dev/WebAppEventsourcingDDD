using WebApplication1.Events;
using System;
using System.Collections.Generic;

namespace WebApplication1
{
    public class AggregateRoot
    {
        // Proprietà principali dell'aggregate
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public int OrderQty { get; set; }
        public DateTime CreatedAt { get; set; }

        // Lista di eventi non commessi
        private readonly List<Event> _uncommittedEvents = new ();
        public IEnumerable<Event> UncommittedEvents => _uncommittedEvents;

        // Aggiungi un evento non commesso e applicalo
        public void AddUncommittedEvent(Event @event)
        {
            _uncommittedEvents.Add(@event);
            // Applicare l'evento subito può non essere sempre una buona idea
            // Rendi l'Apply separato dalla logica di registrazione
        }

        // Pulisci gli eventi non commessi
        public void ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }

        // Applicazione degli eventi specifici
        private void Apply(OrderCreated orderCreated)
        {
            OrderId = orderCreated.OrderId;
            CustomerId = orderCreated.CustomerId;
            OrderQty = orderCreated.OrderQty;
            CreatedAt = orderCreated.CreatedAt;
        }

        private void Apply(OrderUpdated orderUpdated)
        {
            OrderId = orderUpdated.OrderId;
            CustomerId = orderUpdated.CustomerId;
            OrderQty = orderUpdated.OrderQty;
        }
        
        // Metodo per applicare eventi generici
        public void Apply(Event @event)
        {
            switch (@event)
            {
                case OrderCreated orderCreated:
                    Apply(orderCreated);
                    break;

                case OrderUpdated orderUpdated:
                    Apply(orderUpdated);
                    break;

                default:
                    throw new InvalidOperationException($"Event type {@event.GetType().Name} is not recognized.");
            }
        }
    }
}

using System.Collections.Specialized;

namespace WebApplication1.Events;

public class OrderCreated:Event
{
    public Guid OrderId { get; init; }
    public int OrderQty { get; init; }
    public override Guid StreamId => OrderId;
}
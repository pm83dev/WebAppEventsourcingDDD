namespace WebApplication1.Events;

public class OrderUpdated : Event
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public int OrderQty { get; init; }
    public override Guid StreamId => OrderId;
}
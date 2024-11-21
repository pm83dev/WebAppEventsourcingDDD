using System.Text.Json.Serialization;

namespace WebApplication1.Events;

[JsonPolymorphic]
[JsonDerivedType(typeof(OrderCreated), nameof(OrderCreated))]
[JsonDerivedType(typeof(OrderUpdated), nameof(OrderUpdated))]

public abstract class Event
{
    public abstract Guid StreamId { get; }
    public DateTime CreatedAt { get; set; }
}
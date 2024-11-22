using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Events;

[BsonDiscriminator("OrderUpdated")]
public class OrderUpdated : Event
{
    [BsonElement("OrderId")]
    public Guid OrderId { get; init; }
    
    [BsonElement("OrderQty")]
    public int OrderQty { get; init; }
    public override Guid StreamId => OrderId;
}
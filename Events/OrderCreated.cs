using System.Collections.Specialized;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Events;

[BsonDiscriminator("OrderCreated")]
public class OrderCreated:Event
{
    [BsonElement("OrderId")]
    public Guid OrderId { get; init; }
    
    [BsonElement("OrderQty")]
    public int OrderQty { get; init; }
    public override Guid StreamId => OrderId;
}
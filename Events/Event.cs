using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Events;

//confJson
[JsonPolymorphic]
[JsonDerivedType(typeof(OrderCreated), nameof(OrderCreated))]
[JsonDerivedType(typeof(OrderUpdated), nameof(OrderUpdated))]

//confMongo
[BsonDiscriminator(Required = true)]
[BsonKnownTypes(typeof(OrderCreated), typeof(OrderUpdated))]
public abstract class Event
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)] 
    public string Id { get; set; }

    [BsonElement("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    public abstract Guid StreamId { get; } // Rappresenta l'ID ordine
}

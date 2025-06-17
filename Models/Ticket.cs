using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using SampleMvcApp.Models; // Replace with your actual namespace where Message is defined

public class Ticket
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("department")]
    public string? Department { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("status")]
    public string? Status { get; set; }

    [BsonElement("createdBy")]
    public string? CreatedBy { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("priority")]
    public string? Priority { get; set; }

    // Store message references or embed full messages (recommended for small volume)
    [BsonIgnoreIfNull]
    [BsonElement("messages")]
    public List<Message>? Messages { get; set; } = new();
}

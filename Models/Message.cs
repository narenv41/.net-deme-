using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SampleMvcApp.Models
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;
        public string TicketId { get; set; } = null!;
        public string Sender { get; set; } = "";
        public string Text { get; set; } = "";
        public DateTime Timestamp { get; set; }

        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public byte[]? FileData { get; set; }
    }
}

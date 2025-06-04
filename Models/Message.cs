using System;


public class Message
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string? Sender { get; set; }           // Nullable if Sender can be NULL
    public string? Text { get; set; }             // Nullable if Text can be NULL
    public DateTime Timestamp { get; set; }

    public byte[]? FileData { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
}
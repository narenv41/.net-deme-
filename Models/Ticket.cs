using System;
using System.Collections.Generic;
public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;   // if Title cannot be null in DB
    public string? Department { get; set; }      // Nullable if Department can be NULL
    public string? Description { get; set; }     // Nullable if Description can be NULL
    public string? Status { get; set; }          // Nullable if Status can be NULL

    public string? CreatedBy { get; set; }       // Nullable if possible null
    public DateTime CreatedAt { get; set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
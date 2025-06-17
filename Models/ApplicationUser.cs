using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

public class ApplicationUser : IdentityUser
{
    [BsonIgnoreIfNull]
    public List<string>? AssignedTicketIds { get; set; } = new();
}

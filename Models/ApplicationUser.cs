using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
public class ApplicationUser : IdentityUser
{
    public List<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
}
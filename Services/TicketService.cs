using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;
using SampleMvcApp.Models;


namespace SampleMvcApp.Services
{
    public class TicketService
    
    {
        private readonly IMongoCollection<Ticket> _tickets;
        private readonly IMongoCollection<Message> _messages;


        public TicketService(IMongoDatabase database)
        {
            _tickets = database.GetCollection<Ticket>("Tickets");
            _messages = database.GetCollection<Message>("Messages");

        }

        public async Task<List<Ticket>> GetAllAsync() =>
            await _tickets.Find(_ => true).ToListAsync();

        public async Task<Ticket?> GetByIdAsync(string id) =>
            await _tickets.Find(t => t.Id == id).FirstOrDefaultAsync();

        public async Task<Ticket> GetByIdWithMessagesAsync(string id)
{
    var ticket = await _tickets.Find(t => t.Id == id).FirstOrDefaultAsync();
    if (ticket != null)
    {
        // Lookup messages from separate collection
        var messages = await _messages.Find(m => m.TicketId == id).SortBy(m => m.Timestamp).ToListAsync();
        ticket.Messages = messages; // Assign manually
    }

    return ticket;
}

        
        public async Task CreateAsync(Ticket ticket) =>
            await _tickets.InsertOneAsync(ticket);

        public async Task UpdateAsync(string id, Ticket ticket) =>
            await _tickets.ReplaceOneAsync(t => t.Id == id, ticket);

        public async Task<int> CountByStatusAsync(string status)
{
    var filter = Builders<Ticket>.Filter.Eq(t => t.Status, status);
    var count = await _tickets.CountDocumentsAsync(filter);
    return (int)count;
}


    }
}

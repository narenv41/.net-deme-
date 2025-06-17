using MongoDB.Driver;
using SampleMvcApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace SampleMvcApp.Services
{
    public class MessageService
    {
        private readonly IMongoCollection<Message> _messages;

        public MessageService(IMongoDatabase database)
        {
            _messages = database.GetCollection<Message>("Messages");
        }

        public async Task CreateAsync(Message message) =>
            await _messages.InsertOneAsync(message);

        public async Task<Message?> GetByIdAsync(string id) =>
            await _messages.Find(m => m.Id == id).FirstOrDefaultAsync();
    }
}

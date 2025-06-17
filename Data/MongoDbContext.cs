using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SampleMvcApp.Models;

namespace SampleMvcApp.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Ticket> Tickets => _database.GetCollection<Ticket>("Tickets");
        public IMongoCollection<Message> Messages => _database.GetCollection<Message>("Messages");
    }
}

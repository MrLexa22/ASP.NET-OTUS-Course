using MongoDB.Driver;
using Pcf.Administration.Core.Domain.Administration;

namespace Pcf.Administration.DataAccess.MongoDb
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<Employee> Employees => _database.GetCollection<Employee>("employees");
        public IMongoCollection<Role> Roles => _database.GetCollection<Role>("roles");

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}
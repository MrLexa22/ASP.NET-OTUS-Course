using MongoDB.Driver;
using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.DataAccess.MongoDb
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<Preference> Preferences => _database.GetCollection<Preference>("preferences");
        public IMongoCollection<Customer> Customers => _database.GetCollection<Customer>("customers");
        public IMongoCollection<PromoCode> PromoCodes => _database.GetCollection<PromoCode>("promocodes");
        public IMongoCollection<CustomerPreference> CustomerPreferences => _database.GetCollection<CustomerPreference>("customerpreferences");
        public IMongoCollection<PromoCodeCustomer> PromoCodeCustomers => _database.GetCollection<PromoCodeCustomer>("promocodecustomers");

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}

using Pcf.GivingToCustomer.DataAccess.MongoDb;
using System.Linq;
using MongoDB.Driver;

namespace Pcf.GivingToCustomer.DataAccess.Data
{
    public class MongoDbInitializer : IDbInitializer
    {
        private readonly MongoDbContext _context;

        public MongoDbInitializer(MongoDbContext context)
        {
            _context = context;
        }

        public void InitializeDb()
        {
            // Очистка базы данных
            _context.Preferences.DeleteMany(_ => true);
            _context.Customers.DeleteMany(_ => true);

            _context.Preferences.InsertMany(FakeDataFactory.Preferences);

            _context.Customers.InsertMany(FakeDataFactory.Customers);
        }
    }
}

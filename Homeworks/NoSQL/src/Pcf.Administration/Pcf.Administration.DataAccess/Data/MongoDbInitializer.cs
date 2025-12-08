using System.Linq;
using MongoDB.Driver;
using Pcf.Administration.DataAccess.MongoDb;

namespace Pcf.Administration.DataAccess.Data
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
            _context.Roles.DeleteMany(_ => true);
            _context.Employees.DeleteMany(_ => true);

            // Добавление ролей
            _context.Roles.InsertMany(FakeDataFactory.Roles);

            // Добавление сотрудников
            _context.Employees.InsertMany(FakeDataFactory.Employees);
        }
    }
}
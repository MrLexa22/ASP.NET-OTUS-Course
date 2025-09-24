using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.DataAccess.Data
{
    public class EfDbInitializer : IDbInitializer
    {
        private readonly DataContext _dataContext;
        private static bool _alreadyInitialized;

        public EfDbInitializer(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void InitializeDb()
        {
            // Применяем все существующие миграции
            _dataContext.Database.Migrate();

            // Если уже сидировано (есть роли) — ничего не делаем
            if (_dataContext.Roles.Any())
                return;

            SeedRoles();
            SeedPreferences();
            SeedCustomers();
            SeedCustomerPreferences();
            SeedEmployees();
            SeedPromoCodes();

            _alreadyInitialized = true;
        }

        private void RecreateDatabase()
        {
            try { _dataContext.Database.EnsureDeleted(); }
            catch (Exception ex) { Console.WriteLine("[WARN] EnsureDeleted failed: " + ex.Message); }

            _dataContext.ChangeTracker.Clear();
            _dataContext.Database.EnsureCreated();
        }

        private void SeedRoles()
        {
            var roles = FakeDataFactory.Roles
                .Select(r => new Role
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                })
                .ToList();

            _dataContext.Roles.AddRange(roles);
            _dataContext.SaveChanges();
        }

        private void SeedPreferences()
        {
            // Клонируем без навигаций (чтобы не затащить CustomerPreferences / PromoCodes -> Customers)
            var prefs = FakeDataFactory.Preferences
                .Select(p => new Preference
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToList();

            _dataContext.Preferences.AddRange(prefs);
            _dataContext.SaveChanges();
        }

        private void SeedCustomers()
        {
            // В этот момент трекеров Customers быть не должно
            Console.WriteLine("Tracked Customers before add: " +
                _dataContext.ChangeTracker.Entries<Customer>().Count());

            var customers = FakeDataFactory.Customers
                .GroupBy(c => c.Id)
                .Select(g => g.First())
                .Select(c => new Customer
                {
                    Id = c.Id,
                    Email = c.Email,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    PromoCodes = new List<PromoCode>(),
                    CustomerPreferences = new List<CustomerPreference>()
                })
                .ToList();

            _dataContext.Customers.AddRange(customers);
            _dataContext.SaveChanges();
        }

        private void SeedCustomerPreferences()
        {
            if (FakeDataFactory.CustomerPreferences == null || FakeDataFactory.CustomerPreferences.Count == 0)
                return;

            var joins = FakeDataFactory.CustomerPreferences
                .Select(cp => new CustomerPreference
                {
                    Id = cp.Id == Guid.Empty ? Guid.NewGuid() : cp.Id,
                    CustomerId = cp.CustomerId != Guid.Empty
                        ? cp.CustomerId
                        : (cp.Customer?.Id ?? Guid.Empty),
                    PreferenceId = cp.PreferenceId != Guid.Empty
                        ? cp.PreferenceId
                        : (cp.Preference?.Id ?? Guid.Empty)
                })
                .Where(j => j.CustomerId != Guid.Empty && j.PreferenceId != Guid.Empty)
                .ToList();

            _dataContext.CustomerPreferences.AddRange(joins);
            _dataContext.SaveChanges();
        }

        private void SeedEmployees()
        {
            var employeeRoleMap = FakeDataFactory.Employees
                .Select(e => new
                {
                    Employee = new Employee
                    {
                        Id = e.Id,
                        Email = e.Email,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        AppliedPromocodesCount = e.AppliedPromocodesCount,
                        Roles = new List<Role>()
                    },
                    RoleIds = (e.Roles ?? new List<Role>())
                        .Select(r => r.Id)
                        .Distinct()
                        .ToList()
                })
                .ToList();

            _dataContext.Employees.AddRange(employeeRoleMap.Select(x => x.Employee));
            _dataContext.SaveChanges();

            foreach (var map in employeeRoleMap)
            {
                if (map.RoleIds.Count == 0) continue;

                var tracked = _dataContext.Employees
                    .Include(e => e.Roles)
                    .First(e => e.Id == map.Employee.Id);

                var attachRoles = _dataContext.Roles
                    .Where(r => map.RoleIds.Contains(r.Id))
                    .ToList();

                foreach (var r in attachRoles)
                    if (!tracked.Roles.Any(x => x.Id == r.Id))
                        tracked.Roles.Add(r);
            }

            _dataContext.SaveChanges();
        }

        private void SeedPromoCodes()
        {
            if (FakeDataFactory.PromoCodes == null || FakeDataFactory.PromoCodes.Count == 0)
                return;

            var codes = FakeDataFactory.PromoCodes
                .Select(pc => new PromoCode
                {
                    Id = pc.Id == Guid.Empty ? Guid.NewGuid() : pc.Id,
                    Code = pc.Code,
                    ServiceInfo = pc.ServiceInfo,
                    BeginDate = pc.BeginDate,
                    EndDate = pc.EndDate,
                    PartnerName = pc.PartnerName,
                    PreferenceId = pc.PreferenceId != Guid.Empty
                        ? pc.PreferenceId
                        : (pc.Preference?.Id ?? Guid.Empty),
                    CustomerId = pc.CustomerId != Guid.Empty
                        ? pc.CustomerId
                        : (pc.Customer?.Id ?? Guid.Empty)
                })
                .Where(pc => pc.PreferenceId != Guid.Empty && pc.CustomerId != Guid.Empty)
                .ToList();

            _dataContext.PromoCodes.AddRange(codes);
            _dataContext.SaveChanges();
        }
    }
}

using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.DataAccess.Data
{
    public static class FakeDataFactory
    {
        // ---------- Roles ----------
        private static readonly List<Role> _roles = new()
        {
            new Role
            {
                Id = Guid.Parse("53729686-a368-4eeb-8bfa-cc69b6050d02"),
                Name = "Admin",
                Description = "Администратор"
            },
            new Role
            {
                Id = Guid.Parse("b0ae7aac-5493-45cd-ad16-87426a5e7665"),
                Name = "PartnerManager",
                Description = "Партнерский менеджер"
            }
        };
        public static IReadOnlyList<Role> Roles => _roles;

        // ---------- Preferences ----------
        private static readonly List<Preference> _preferences = new()
        {
            new Preference
            {
                Id = Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c"),
                Name = "Театр"
            },
            new Preference
            {
                Id = Guid.Parse("c4bda62e-fc74-4256-a956-4760b3858cbd"),
                Name = "Семья"
            },
            new Preference
            {
                Id = Guid.Parse("76324c47-68d2-472d-abb8-33cfa8cc0c84"),
                Name = "Дети"
            },
            new Preference
            {
                Id = Guid.Parse("0f9b8aa9-1c8e-4f4f-8f5f-5f7a9d8c0abc"),
                Name = "Путешествия"
            }
        };
        public static IReadOnlyList<Preference> Preferences => _preferences;

        // ---------- Customers ----------
        // Несколько клиентов: с промокодами, без промокодов, с/без предпочтений
        private static readonly List<Customer> _customers = new()
        {
            new Customer
            {
                Id = Guid.Parse("dd43460a-ae9f-413c-ad44-f99e36c8b79a"),
                Email = "ivan.petrov@example.com",
                FirstName = "Иван",
                LastName = "Петров",
                CustomerPreferences = new List<CustomerPreference>(),
                PromoCodes = new List<PromoCode>()
            },
            new Customer
            {
                Id = Guid.Parse("4d5e3f1b-6d63-44fa-9f1a-1b4f5e8c1f22"),
                Email = "olga.smirnova@example.com",
                FirstName = "Ольга",
                LastName = "Смирнова",
                CustomerPreferences = new List<CustomerPreference>(), // будет без предпочтений и без промокодов
                PromoCodes = new List<PromoCode>()
            },
            new Customer
            {
                Id = Guid.Parse("2b1c9d7e-8a44-4c9f-9f1e-223344556677"),
                Email = "sergey.kuznetsov@example.com",
                FirstName = "Сергей",
                LastName = "Кузнецов",
                CustomerPreferences = new List<CustomerPreference>(), // будет с предпочтениями, без промокодов
                PromoCodes = new List<PromoCode>()
            },
            new Customer
            {
                Id = Guid.Parse("7fa2b8e4-5f8e-4f0d-9a2b-8899aa11bb22"),
                Email = "anna.volkova@example.com",
                FirstName = "Анна",
                LastName = "Волкова",
                CustomerPreferences = new List<CustomerPreference>(), // с предпочтениями и промокодами
                PromoCodes = new List<PromoCode>()
            }
        };
        public static IReadOnlyList<Customer> Customers => _customers;

        // ---------- Join CustomerPreferences ----------
        private static readonly List<CustomerPreference> _customerPreferences;
        public static IReadOnlyList<CustomerPreference> CustomerPreferences => _customerPreferences;

        // ---------- Employees ----------
        private static readonly List<Employee> _employees = new()
        {
            new Employee
            {
                Id = Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
                Email = "owner@somemail.ru",
                FirstName = "Иван",
                LastName = "Сергеев",
                Roles = new List<Role> { _roles.First(r => r.Name == "Admin") },
                AppliedPromocodesCount = 5
            },
            new Employee
            {
                Id = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895"),
                Email = "andreev@somemail.ru",
                FirstName = "Петр",
                LastName = "Андреев",
                Roles = new List<Role> { _roles.First(r => r.Name == "PartnerManager") },
                AppliedPromocodesCount = 10
            }
        };
        public static IReadOnlyList<Employee> Employees => _employees;

        // ---------- PromoCodes ----------
        private static readonly List<PromoCode> _promoCodes = new();
        public static IReadOnlyList<PromoCode> PromoCodes => _promoCodes;

        // ---------- Static ctor: связываем данные ----------
        static FakeDataFactory()
        {
            var customer1 = _customers[0]; // Иван Петров (получит промокоды и предпочтения)
            var customer2 = _customers[1]; // Ольга Смирнова (без промокодов и предпочтений)
            var customer3 = _customers[2]; // Сергей Кузнецов (только предпочтения)
            var customer4 = _customers[3]; // Анна Волкова (предпочтения + промокоды)

            var prefTheatre = _preferences.First(p => p.Name == "Театр");
            var prefFamily = _preferences.First(p => p.Name == "Семья");
            var prefKids = _preferences.First(p => p.Name == "Дети");
            var prefTravel = _preferences.First(p => p.Name == "Путешествия");

            // Join предпочтений
            _customerPreferences = new List<CustomerPreference>
            {
                // Иван: Театр + Семья
                new CustomerPreference
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer1.Id,
                    Customer = customer1,
                    PreferenceId = prefTheatre.Id,
                    Preference = prefTheatre
                },
                new CustomerPreference
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer1.Id,
                    Customer = customer1,
                    PreferenceId = prefFamily.Id,
                    Preference = prefFamily
                },
                // Сергей: Дети + Путешествия
                new CustomerPreference
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer3.Id,
                    Customer = customer3,
                    PreferenceId = prefKids.Id,
                    Preference = prefKids
                },
                new CustomerPreference
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer3.Id,
                    Customer = customer3,
                    PreferenceId = prefTravel.Id,
                    Preference = prefTravel
                },
                // Анна: Театр + Путешествия
                new CustomerPreference
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer4.Id,
                    Customer = customer4,
                    PreferenceId = prefTheatre.Id,
                    Preference = prefTheatre
                },
                new CustomerPreference
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer4.Id,
                    Customer = customer4,
                    PreferenceId = prefTravel.Id,
                    Preference = prefTravel
                }
            };

            // Разносим обратные навигации
            foreach (var cp in _customerPreferences)
            {
                var cust = _customers.First(c => c.Id == cp.CustomerId);
                cust.CustomerPreferences.Add(cp);

                var pref = _preferences.First(p => p.Id == cp.PreferenceId);
                pref.CustomerPreferences ??= new List<CustomerPreference>();
                pref.CustomerPreferences.Add(cp);
            }

            // Промокоды (у Ивана и Анны; у Ольги нет; у Сергея нет)
            var now = DateTime.UtcNow.Date;
            _promoCodes.AddRange(new[]
            {
                new PromoCode
                {
                    Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                    Code = "WELCOME-IVAN-01",
                    ServiceInfo = "Скидка 10% на театр",
                    BeginDate = now.AddDays(-10),
                    EndDate = now.AddDays(20),
                    PartnerName = "TheatreCorp",
                    PreferenceId = prefTheatre.Id,
                    Preference = prefTheatre,
                    CustomerId = customer1.Id,
                    Customer = customer1
                },
                new PromoCode
                {
                    Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                    Code = "FAMILY-IVAN-02",
                    ServiceInfo = "Пакет для семьи",
                    BeginDate = now.AddDays(-5),
                    EndDate = now.AddDays(30),
                    PartnerName = "FamilyService",
                    PreferenceId = prefFamily.Id,
                    Preference = prefFamily,
                    CustomerId = customer1.Id,
                    Customer = customer1
                },
                new PromoCode
                {
                    Id = Guid.Parse("a3333333-3333-3333-3333-333333333333"),
                    Code = "TRAVEL-ANNA-01",
                    ServiceInfo = "Бонус путешествия",
                    BeginDate = now,
                    EndDate = now.AddDays(60),
                    PartnerName = "TravelPlus",
                    PreferenceId = prefTravel.Id,
                    Preference = prefTravel,
                    CustomerId = customer4.Id,
                    Customer = customer4
                }
            });

            // Проставляем навигации PromoCodes в клиентов + в Preferences
            foreach (var pc in _promoCodes)
            {
                var cust = _customers.First(c => c.Id == pc.CustomerId);
                cust.PromoCodes.Add(pc);

                var pref = _preferences.First(p => p.Id == pc.PreferenceId);
                pref.PromoCodes ??= new List<PromoCode>();
                pref.PromoCodes.Add(pc);
            }
        }
    }
}
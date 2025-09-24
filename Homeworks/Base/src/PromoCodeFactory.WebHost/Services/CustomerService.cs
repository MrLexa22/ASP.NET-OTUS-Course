using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.ExceptionHandling.Exceptions;
using PromoCodeFactory.WebHost.Mappers;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Parsers;
using PromoCodeFactory.WebHost.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Preference> _preferenceRepository;
        public CustomerService(IRepository<Customer> customerRepository, IRepository<Preference> preferenceRepository)
        {
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
        }

        public async Task<Guid> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            try
            {
                if (request == null)
                    throw new ValidationException("Request is null");

                if (string.IsNullOrWhiteSpace(request.Email))
                    throw new ValidationException("Email is empty");

                if (string.IsNullOrWhiteSpace(request.FirstName))
                    throw new ValidationException("FirstName is empty");

                if (string.IsNullOrWhiteSpace(request.LastName))
                    throw new ValidationException("LastName is empty");

                if (request.PreferenceIds == null || request.PreferenceIds.Count == 0)
                    throw new ValidationException("Preferences is empty");

                var distinctPreferenceIds = request.PreferenceIds
                    .Where(id => id != Guid.Empty)
                    .Distinct()
                    .ToList();

                if (distinctPreferenceIds.Count == 0)
                    throw new ValidationException("All preference ids are empty");

                // Проверка уникальности Email
                var existing = await _customerRepository.FindAsync(c => c.Email == request.Email.Trim());
                if (existing.Any())
                    throw new ValidationException("Customer with email already exist");

                // Получаем все нужные предпочтения
                var allPreferences = await _preferenceRepository.FindAsync(p => distinctPreferenceIds.Contains(p.Id));
                var found = allPreferences.ToList();
                var notFound = distinctPreferenceIds.Except(found.Select(p => p.Id)).ToList();
                if (notFound.Any())
                    throw new ValidationException($"Preference(s) not found: {string.Join(", ", notFound)}");

                // Формируем сущность
                var newCustomerId = Guid.NewGuid();
                var customerPreferences = found
                    .Select(pref => new CustomerPreference
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = newCustomerId,
                        PreferenceId = pref.Id
                    })
                    .ToList();

                var customer = new Customer
                {
                    Id = newCustomerId,
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    Email = request.Email.Trim(),
                    CustomerPreferences = customerPreferences,
                    PromoCodes = new List<PromoCode>()
                };

                // Сохранение
                var created = await _customerRepository.CreateAsync(customer);
                return created.Id;
            }
            catch(Exception ex1)
            {
                return Guid.Empty;
            }
        }

        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            // Проверка Id
            if (id == Guid.Empty)
                throw new ValidationException("Id is empty");

            // Проверка, что пользователь существует
            Customer findCustomer = await _customerRepository.GetByIdAsync(id);
            if (findCustomer == null)
                throw new NotFoundEntityException("Customer", id);

            bool result = await _customerRepository.DeleteAsync(findCustomer);

            return result;
        }

        public async Task<CustomerResponse> GetCustomerByIdAsync(Guid id)
        {
            //var customer = await _customerRepository.GetByIdAsync(id);
            var customer = await _customerRepository.GetSingleAsync(
                e => e.Id == id,
                q => q.Include(e => e.PromoCodes).Include(e => e.CustomerPreferences)//.ThenInclude(p => p.Preference)
            );

            return CustomerMapper.ToResponse(customer);
        }

        public async Task<List<CustomerShortResponse>> GetCustomers()
        {
            var customers = await _customerRepository.GetAllAsync();
            return CustomerMapper.ToShort(customers);
        }

        public async Task<CustomerResponse> UpdateCustomerAsync(CreateOrEditCustomerRequest request, Guid id)
        {
            if (request == null)
                throw new ValidationException("Request is null");
            if (id == Guid.Empty)
                throw new ValidationException("Id is empty");
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ValidationException("Email is empty");
            if (string.IsNullOrWhiteSpace(request.FirstName))
                throw new ValidationException("FirstName is empty");
            if (string.IsNullOrWhiteSpace(request.LastName))
                throw new ValidationException("LastName is empty");
            if (request.PreferenceIds == null || request.PreferenceIds.Count == 0)
                throw new ValidationException("Preferences is empty");

            var distinctPrefIds = request.PreferenceIds
                .Where(g => g != Guid.Empty)
                .Distinct()
                .ToList();
            if (distinctPrefIds.Count == 0)
                throw new ValidationException("All preference ids are empty");

            // Загружаем существующего клиента с навигациями (tracking!)
            // Используем include через конкретный контекст (обходя AsNoTracking)
            var existing = await _customerRepository.GetByIdAsync(
                id,
                q => q.Include(c => c.CustomerPreferences)
                      .ThenInclude(cp => cp.Preference),
                asNoTracking: false);

            if (existing == null)
                throw new NotFoundEntityException("Customer", id);

            // Проверка Email уникальности
            var emailDuplicate = await _customerRepository.FindAsync(c => c.Email == request.Email.Trim() && c.Id != id);
            if (emailDuplicate.Any())
                throw new ValidationException("Customer with email already exist");

            // Проверка предпочтений
            var foundPrefs = await _preferenceRepository.FindAsync(p => distinctPrefIds.Contains(p.Id));
            var foundList = foundPrefs.ToList();
            var notFound = distinctPrefIds.Except(foundList.Select(p => p.Id)).ToList();
            if (notFound.Any())
                throw new ValidationException($"Preference(s) not found: {string.Join(", ", notFound)}");

            // Обновляем простые поля
            existing.FirstName = request.FirstName.Trim();
            existing.LastName = request.LastName.Trim();
            existing.Email = request.Email.Trim();

            // Синхронизация CustomerPreferences
            var currentPrefIds = existing.CustomerPreferences?.Select(cp => cp.PreferenceId).ToHashSet() ?? new HashSet<Guid>();
            var newPrefIds = distinctPrefIds.ToHashSet();

            // Удаляем лишние
            var toRemove = existing.CustomerPreferences
                .Where(cp => !newPrefIds.Contains(cp.PreferenceId))
                .ToList();
            foreach (var rem in toRemove)
                existing.CustomerPreferences.Remove(rem);

            // Добавляем недостающие
            var toAddIds = newPrefIds.Except(currentPrefIds).ToList();
            foreach (var addId in toAddIds)
            {
                existing.CustomerPreferences.Add(new CustomerPreference
                {
                    Id = Guid.NewGuid(),
                    CustomerId = existing.Id,
                    PreferenceId = addId
                });
            }

            // Обновляем (репозиторий сам сохранит)
            await _customerRepository.UpdateAsync(existing);

            // Повторная загрузка для ответа
            var reloaded = await _customerRepository.GetByIdAsync(
                id,
                q => q.Include(c => c.PromoCodes)
                      .Include(c => c.CustomerPreferences).ThenInclude(cp => cp.Preference),
                asNoTracking: true);

            return CustomerMapper.ToResponse(reloaded);
        }
    }
}

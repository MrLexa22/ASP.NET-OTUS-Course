using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Mappers;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Services
{
    public class PromocodesService : IPromocodeService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Preference> _preferenceRepository;
        private readonly IRepository<PromoCode> _promocodeRepository;
        public PromocodesService(IRepository<Customer> customerRepository, IRepository<PromoCode> promocodeRepository, IRepository<Preference> preferenceRepository)
        {
            _customerRepository = customerRepository;
            _promocodeRepository = promocodeRepository;
            _preferenceRepository = preferenceRepository;
        }

        public async Task<List<PromoCodeShortResponse>> GetListPromocodesShortResponseAsync()
        {
            var listPromocodes = await _promocodeRepository.GetAllAsync();
            return PromocodeMapper.ToShort(listPromocodes.ToList());
        }

        public async Task<bool> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            var preference = await _preferenceRepository.GetByIdAsync(Guid.Parse(request.Preference));
            if(preference == null)
            {
                return false;
            }

            var customers = await _customerRepository.FindAsync(c => c.CustomerPreferences.Any(p => p.PreferenceId == preference.Id));
            if(customers == null)
            {
                return false;
            }

            foreach(var customer in customers)
            {
                await _promocodeRepository.CreateAsync(new PromoCode() 
                {
                    Code = request.PromoCode,
                    ServiceInfo = request.ServiceInfo,
                    BeginDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(14),
                    PartnerName = request.PartnerName,
                    PreferenceId = preference.Id,
                    CustomerId = customer.Id
                });
            }

            return true;
        }
    }
}

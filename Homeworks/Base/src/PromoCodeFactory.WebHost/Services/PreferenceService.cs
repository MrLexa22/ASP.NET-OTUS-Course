using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Mappers;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Services
{
    public class PreferenceService : IPreferenceService
    {
        private readonly IRepository<Preference> _preferenceRepository;
        public PreferenceService(IRepository<Customer> customerRepository, IRepository<Preference> preferenceRepository)
        {
            _preferenceRepository = preferenceRepository;
        }
        public async Task<List<PrefernceResponse>> GetPreferences()
        {
            return PreferenceMapper.ToResponse(await _preferenceRepository.GetAllAsync());
        }
    }
}

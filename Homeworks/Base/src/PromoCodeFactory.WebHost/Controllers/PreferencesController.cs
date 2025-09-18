using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Services;
using PromoCodeFactory.WebHost.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Предпочтения
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController : ControllerBase
    {
        private readonly IPreferenceService _preferenceService;
        public PreferencesController(IPreferenceService preferenceService)
        {
            _preferenceService = preferenceService;
        }

        /// <summary>
        /// Получить данные всех предпочтений
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<PrefernceResponse>> GetPreferencesAsync()
        {
            var preferences = await _preferenceService.GetPreferences();
            return preferences;
        }
    }
}

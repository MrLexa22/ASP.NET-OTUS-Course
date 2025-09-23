using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Services;
using PromoCodeFactory.WebHost.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Управление предпочтениями (чтение).
    /// </summary>
    /// <remarks>
    /// Базовый маршрут: <c>api/v1/preferences</c>. Все ответы — в формате JSON.
    /// </remarks>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class PreferencesController : ControllerBase
    {
        private readonly IPreferenceService _preferenceService;

        /// <summary>
        /// Инициализирует контроллер предпочтений.
        /// </summary>
        /// <param name="preferenceService">Сервис работы с предпочтениями.</param>
        public PreferencesController(IPreferenceService preferenceService)
        {
            _preferenceService = preferenceService;
        }

        /// <summary>
        /// Получить список предпочтений.
        /// </summary>
        /// <remarks>
        /// Возвращает коллекцию с идентификатором и названием предпочтения.
        /// </remarks>
        /// <returns>Список моделей предпочтений.</returns>
        /// <response code="200">Список успешно получен.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PrefernceResponse>))]
        public async Task<List<PrefernceResponse>> GetPreferencesAsync()
        {
            var preferences = await _preferenceService.GetPreferences();
            return preferences;
        }
    }
}

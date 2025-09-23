using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Управление промокодами (получение и выдача клиентам).
    /// </summary>
    /// <remarks>
    /// Базовый маршрут: <c>api/v1/promocodes</c>. Все ответы — в формате JSON.
    /// </remarks>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class PromocodesController
        : ControllerBase
    {
        private readonly IPromocodeService _promocodeService;
        public PromocodesController(IPromocodeService promocodeService)
        {
            _promocodeService = promocodeService;
        }

        /// <summary>
        /// Получить список всех промокодов (краткая информация).
        /// </summary>
        /// <remarks>
        /// В ответе содержатся поля: <c>Id</c>, <c>Code</c>, <c>ServiceInfo</c>, <c>BeginDate</c>, <c>EndDate</c>, <c>PartnerName</c>.
        /// Даты представлены строками (ISO 8601).
        /// </remarks>
        /// <returns>Список кратких моделей промокодов.</returns>
        /// <response code="200">Список успешно получен.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PromoCodeShortResponse>))]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            var listPromocodes = await _promocodeService.GetListPromocodesShortResponseAsync();
            return Ok(listPromocodes);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением.
        /// </summary>
        /// <param name="request">
        /// Параметры создания и выдачи промокода:
        /// <list type="bullet">
        /// <item><description><c>ServiceInfo</c> — описание услуги/предложения.</description></item>
        /// <item><description><c>PartnerName</c> — название партнёра.</description></item>
        /// <item><description><c>PromoCode</c> — значение промокода.</description></item>
        /// <item><description><c>Preference</c> — предпочтение клиентов, которым будет выдан промокод.</description></item>
        /// </list>
        /// </param>
        /// <returns>Результат выполнения операции выдачи промокодов.</returns>
        /// <response code="200">Промокод создан и выдан подходящим клиентам.</response>
        /// <response code="400">Ошибка валидации входных данных или операция невозможна.</response>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            var result = await _promocodeService.GivePromoCodesToCustomersWithPreferenceAsync(request);
            if (result)
                return Ok(result);
            else
                return BadRequest(result);
        }
    }
}
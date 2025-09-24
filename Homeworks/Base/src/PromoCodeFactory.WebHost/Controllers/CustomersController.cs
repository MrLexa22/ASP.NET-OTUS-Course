using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Services;
using PromoCodeFactory.WebHost.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Управление покупателями (чтение, создание, изменение и удаление).
    /// </summary>
    /// <remarks>
    /// Базовый маршрут: <c>api/v1/customers</c>.
    /// Все ответы возвращаются в формате JSON.
    /// </remarks>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class CustomersController
        : ControllerBase
    {
        private readonly ICustomerService _customerService;

        /// <summary>
        /// Инициализирует контроллер покупателей.
        /// </summary>
        /// <param name="customerService">Сервис работы с покупателями.</param>
        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Получить список покупателей (краткая информация).
        /// </summary>
        /// <remarks>
        /// Возвращает коллекцию с идентификатором, именем, фамилией и e-mail покупателя.
        /// </remarks>
        /// <returns>Список кратких моделей покупателей.</returns>
        /// <response code="200">Список успешно получен.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CustomerShortResponse>))]
        public async Task<ActionResult<List<CustomerShortResponse>>> GetCustomersAsync()
        {
            var customers = await _customerService.GetCustomers();
            return customers;
        }

        /// <summary>
        /// Получить подробные данные покупателя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор покупателя.</param>
        /// <returns>Подробная модель покупателя с промокодами и предпочтениями.</returns>
        /// <response code="200">Данные покупателя найдены и возвращены.</response>
        /// <response code="204">Покупатель не найден.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerResponse))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            return customer;
        }

        /// <summary>
        /// Создать нового покупателя.
        /// </summary>
        /// <param name="request">
        /// Данные покупателя: имя, фамилия, e-mail и список идентификаторов предпочтений.
        /// </param>
        /// <returns>Идентификатор созданного покупателя.</returns>
        /// <response code="200">Покупатель успешно создан, возвращён его идентификатор.</response>
        /// <response code="400">Ошибка валидации или невозможно создать покупателя.</response>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            var customerIdNew = await _customerService.CreateCustomerAsync(request);

            if(customerIdNew == Guid.Empty)
                return BadRequest("Can't create customer");

            return customerIdNew;
        }

        /// <summary>
        /// Обновить данные покупателя.
        /// </summary>
        /// <param name="id">Идентификатор покупателя.</param>
        /// <param name="request">
        /// Новые данные покупателя: имя, фамилия, e-mail и список идентификаторов предпочтений.
        /// </param>
        /// <returns>Обновлённая модель покупателя.</returns>
        /// <response code="200">Покупатель успешно обновлён.</response>
        /// <response code="400">Ошибка валидации или невозможно обновить покупателя.</response>
        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CustomerResponse>> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            var customerNew = await _customerService.UpdateCustomerAsync(request, id);

            if (customerNew == null)
                return BadRequest("Can't update customer");

            return customerNew;
        }

        /// <summary>
        /// Удалить покупателя.
        /// </summary>
        /// <param name="id">Идентификатор покупателя. Передаётся как параметр строки запроса.</param>
        /// <returns>
        /// Признак успешности операции:
        /// <c>true</c> — покупатель удалён; <c>false</c> — покупатель не найден или не удалён.
        /// </returns>
        /// <response code="200">Результат операции удаления возвращён.</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<ActionResult<bool>> DeleteCustomer(Guid id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            return Ok(result);
        }
    }
}
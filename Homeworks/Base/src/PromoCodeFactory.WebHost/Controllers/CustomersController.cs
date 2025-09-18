using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Services;
using PromoCodeFactory.WebHost.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController
        : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Получить покупателей
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<CustomerShortResponse>>> GetCustomersAsync()
        {
            var customers = await _customerService.GetCustomers();
            return customers;
        }

        /// <summary>
        /// Получить данные покупателя по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            return customer;
        }

        /// <summary>
        /// Создание нового покупателя
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            var customerIdNew = await _customerService.CreateCustomerAsync(request);

            if(customerIdNew == Guid.Empty)
                return BadRequest("Can't create customer");

            return customerIdNew;
        }

        /// <summary>
        /// Изменение покупателя
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerResponse>> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            var customerNew = await _customerService.UpdateCustomerAsync(request, id);

            if (customerNew == null)
                return BadRequest("Can't update customer");

            return customerNew;
        }

        /// <summary>
        /// Удаление покупателя
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteCustomer(Guid id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            return Ok(result);
        }
    }
}
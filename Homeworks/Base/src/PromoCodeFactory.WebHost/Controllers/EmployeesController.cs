using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Services.Abstractions;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeService.GetEmployeesAsync();
            return employees;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            return employee;
        }

        /// <summary>
        /// Создание нового сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateEmployeeAsync(EmployeeRequest request)
        {
            var guidCreatedEmployee = await _employeeService.CreateEmployeeAsync(request);
            return Ok(guidCreatedEmployee);
        }

        /// <summary>
        /// Обновление сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync(EmployeeRequest request)
        {
            var updateEmployee = await _employeeService.UpdateEmployeeAsync(request);
            return Ok(updateEmployee);
        }

        /// <summary>
        /// Удаление сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteEmployeeAsync(Guid id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);
            return Ok(result);
        }
    }
}
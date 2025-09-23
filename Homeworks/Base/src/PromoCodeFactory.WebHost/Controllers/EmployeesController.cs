using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Services.Abstractions;
using Microsoft.AspNetCore.Http;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Управление сотрудниками (чтение, создание, изменение и удаление).
    /// </summary>
    /// <remarks>
    /// Базовый маршрут: <c>api/v1/employees</c>. Все ответы — в формате JSON.
    /// </remarks>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Инициализирует контроллер сотрудников.
        /// </summary>
        /// <param name="employeeService">Сервис работы с сотрудниками.</param>
        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Получить список сотрудников (краткая информация).
        /// </summary>
        /// <remarks>
        /// Возвращает идентификатор, полное имя и e-mail сотрудника.
        /// </remarks>
        /// <returns>Список кратких моделей сотрудников.</returns>
        /// <response code="200">Список успешно получен.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EmployeeShortResponse>))]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeService.GetEmployeesAsync();
            return employees;
        }

        /// <summary>
        /// Получить данные сотрудника по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сотрудника.</param>
        /// <returns>Подробная модель сотрудника с ролями и статистикой промокодов.</returns>
        /// <response code="200">Данные сотрудника найдены и возвращены.</response>
        /// <response code="204">Сотрудник не найден.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeeResponse))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            return employee;
        }

        /// <summary>
        /// Создать нового сотрудника.
        /// </summary>
        /// <param name="request">Данные сотрудника: имя, фамилия, e-mail, роли и счётчик применённых промокодов.</param>
        /// <returns>Идентификатор созданного сотрудника.</returns>
        /// <response code="200">Сотрудник успешно создан, возвращён его идентификатор.</response>
        /// <response code="400">Ошибка валидации входной модели.</response>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> CreateEmployeeAsync(EmployeeRequest request)
        {
            var guidCreatedEmployee = await _employeeService.CreateEmployeeAsync(request);
            return Ok(guidCreatedEmployee);
        }

        /// <summary>
        /// Обновить данные сотрудника.
        /// </summary>
        /// <param name="request">Новые данные сотрудника: имя, фамилия, e-mail, роли и счётчик применённых промокодов.</param>
        /// <returns>Обновлённая модель сотрудника.</returns>
        /// <response code="200">Сотрудник успешно обновлён.</response>
        /// <response code="400">Ошибка валидации входной модели.</response>
        [HttpPut]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeeResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync(EmployeeRequest request)
        {
            var updateEmployee = await _employeeService.UpdateEmployeeAsync(request);
            return Ok(updateEmployee);
        }

        /// <summary>
        /// Удалить сотрудника.
        /// </summary>
        /// <param name="id">Идентификатор сотрудника (передаётся как параметр строки запроса).</param>
        /// <returns>
        /// Признак успешности операции:
        /// <c>true</c> — сотрудник удалён; <c>false</c> — сотрудник не найден или не удалён.
        /// </returns>
        /// <response code="200">Результат операции удаления возвращён.</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<ActionResult<bool>> DeleteEmployeeAsync(Guid id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);
            return Ok(result);
        }
    }
}
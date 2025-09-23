using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Services.Abstractions;
using Microsoft.AspNetCore.Http;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Управление ролями сотрудников (чтение).
    /// </summary>
    /// <remarks>
    /// Базовый маршрут: <c>api/v1/roles</c>. Все ответы — в формате JSON.
    /// </remarks>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _rolesService;

        /// <summary>
        /// Инициализирует контроллер ролей.
        /// </summary>
        /// <param name="rolesService">Сервис работы с ролями сотрудников.</param>
        public RolesController(IRoleService rolesService)
        {
            _rolesService = rolesService;
        }

        /// <summary>
        /// Получить все доступные роли сотрудников.
        /// </summary>
        /// <returns>Список ролей с идентификатором, названием и описанием.</returns>
        /// <response code="200">Список успешно получен.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RoleItemResponse>))]
        public async Task<List<RoleItemResponse>> GetRolesAsync()
        {
            var roles = await _rolesService.GetRolesAsync();

            return roles;
        }
    }
}
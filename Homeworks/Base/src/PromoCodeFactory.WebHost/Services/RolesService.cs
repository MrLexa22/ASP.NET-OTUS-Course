using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Mappers;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Services
{
    public class RolesService : IRoleService
    {
        private readonly IRepository<Role> _rolesRepository;

        public RolesService(IRepository<Role> rolesRepository)
        {
            _rolesRepository = rolesRepository;
        }

        /// <summary>
        /// Сервис получения всех ролей
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<RoleItemResponse>> GetRolesAsync()
        {
            var roles = await _rolesRepository.GetAllAsync();

            return RoleMapper.ToRoleItemResponse(roles);
        }
    }
}

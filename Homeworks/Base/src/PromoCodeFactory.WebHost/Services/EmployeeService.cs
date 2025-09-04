using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.ExceptionHandling.Exceptions;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Parsers;
using PromoCodeFactory.WebHost.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Role> _roleRepository;
        public EmployeeService(IRepository<Employee> employeeRepository, IRepository<Role> roleRepository)
        {
            _employeeRepository = employeeRepository;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// Сервис получения списка сотрудников
        /// </summary>
        /// <returns></returns>
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            return EmployeeMapper.ToShort(employees);
        }

        /// <summary>
        /// Сервис получения сотрудника по ID
        /// </summary>
        /// <returns></returns>
        public async Task<EmployeeResponse> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                throw new NotFoundEntityException("Employee", id);

            return EmployeeMapper.ToResponse(employee);
        }

        /// <summary>
        /// Сервис создания сотрудника
        /// </summary>
        /// <returns></returns>
        public async Task<Guid> CreateEmployeeAsync(EmployeeRequest request)
        {
            // Проверка указания ролей
            if (request.Roles == null || request.Roles.Count() <= 0)
                throw new ValidationException("Roles is empty");

            // Проверка, что пользователь с указанным Email не существует
            if ((await _employeeRepository.FindAsync(p => p.Email == request.Email)).Any())
                throw new ValidationException("Employee with email already exist");

            // Проверка, что переданные Guid ролей существуют
            List<Role> roles = (await _roleRepository.GetAllAsync()).ToList();
            var notFoundRoles = request.Roles.Where(roleId => !roles.Any(r => r.Id == roleId)).ToList();
            if (notFoundRoles.Any())
                throw new ValidationException($"Role(s) not found: {string.Join(", ", notFoundRoles)}");

            var employee = EmployeeMapper.ToNewOrUpdateEmployee(request, roles.Where(r => request.Roles.Contains(r.Id)).ToList());

            var created = await _employeeRepository.CreateAsync(employee);
            return created.Id;
        }

        /// <summary>
        /// Сервис обновления сотрудника
        /// </summary>
        /// <returns></returns>
        public async Task<EmployeeResponse> UpdateEmployeeAsync(EmployeeRequest request)
        {
            // Проверка Id
            if (request.Id == null || request.Id == Guid.Empty)
                throw new ValidationException("Id is empty");

            // Проверка, что пользователь существует
            Employee findEmployee = await _employeeRepository.GetByIdAsync((Guid)request.Id);
            if (findEmployee == null)
                throw new NotFoundEntityException("Employee", (Guid)request.Id);

            // Проверка, что пользователь с указанным Email не существует
            if ((await _employeeRepository.FindAsync(p => p.Email == request.Email && p.Id != request.Id)).Any())
                throw new ValidationException("Employee with email already exist");

            // Проверка, что переданные Guid ролей существуют
            List<Role> roles = null;
            if (request.Roles == null || request.Roles.Count() <= 0)
            {
                roles = (await _roleRepository.GetAllAsync()).ToList();
                var notFoundRoles = request.Roles.Where(roleId => !roles.Any(r => r.Id == roleId)).ToList();
                if (notFoundRoles.Any())
                    throw new ValidationException($"Role(s) not found: {string.Join(", ", notFoundRoles)}");
            }

            var employee = EmployeeMapper.ToNewOrUpdateEmployee(
                request, 
                request.Roles?.Count > 0 ? roles.Where(r => request.Roles.Contains(r.Id)).ToList() : findEmployee.Roles, 
                findEmployee.Id);

            var updatedEmployee = await _employeeRepository.UpdateAsync(employee);

            return EmployeeMapper.ToResponse(updatedEmployee);
        }

        /// <summary>
        /// Сервис удаления сотрудника
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteEmployeeAsync(Guid id)
        {
            // Проверка Id
            if (id == Guid.Empty)
                throw new ValidationException("Id is empty");

            // Проверка, что пользователь существует
            Employee findEmployee = await _employeeRepository.GetByIdAsync(id);
            if (findEmployee == null)
                throw new NotFoundEntityException("Employee", id);

            bool result = await _employeeRepository.DeleteAsync(findEmployee);

            return result;
        }
    }
}

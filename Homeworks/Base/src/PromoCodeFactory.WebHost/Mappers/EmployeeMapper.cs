using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.WebHost.Parsers
{
    public static class EmployeeMapper
    {
        public static List<EmployeeShortResponse> ToShort(IEnumerable<Employee> employees)
        {
            return employees.Select(e => ToShort(e)).ToList();
        }

        public static EmployeeShortResponse ToShort(Employee employee) => new()
        {
            Id = employee.Id,
            FullName = employee.FullName,
            Email = employee.Email
        };

        public static EmployeeResponse ToResponse(Employee employee) => new()
        {
            Id = employee.Id,
            FullName = employee.FullName,
            Email = employee.Email,
            Roles = employee.Roles.Select(r => new RoleItemResponse
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            }).ToList(),
            AppliedPromocodesCount = employee.AppliedPromocodesCount
        };

        public static Employee ToNewOrUpdateEmployee(EmployeeRequest employee, List<Role> roles, Guid? id = null) => new()
        {
            Id = id == null ? Guid.NewGuid() : (Guid)id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            AppliedPromocodesCount = employee.AppliedPromocodesCount,
            Roles = roles
        };
    }
}

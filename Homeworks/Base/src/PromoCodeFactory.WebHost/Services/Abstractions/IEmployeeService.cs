using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace PromoCodeFactory.WebHost.Services.Abstractions
{
    public interface IEmployeeService
    {
        Task<Guid> CreateEmployeeAsync(EmployeeRequest request);
        Task<EmployeeResponse> GetEmployeeByIdAsync(Guid id);
        Task<List<EmployeeShortResponse>> GetEmployeesAsync();
        Task<EmployeeResponse> UpdateEmployeeAsync(EmployeeRequest request);
        Task<bool> DeleteEmployeeAsync(Guid id);
    }
}

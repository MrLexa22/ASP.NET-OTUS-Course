using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Services.Abstractions
{
    public interface ICustomerService
    {
        Task<List<CustomerShortResponse>> GetCustomers();
        Task<CustomerResponse> GetCustomerByIdAsync(Guid id);
        Task<Guid> CreateCustomerAsync(CreateOrEditCustomerRequest request);
        Task<CustomerResponse> UpdateCustomerAsync(CreateOrEditCustomerRequest request, Guid id);
        Task<bool> DeleteCustomerAsync(Guid id);
    }
}

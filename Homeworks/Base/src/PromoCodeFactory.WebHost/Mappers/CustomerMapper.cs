using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.WebHost.Mappers
{
    public static class CustomerMapper
    {
        public static CustomerResponse ToResponse(Customer customer) => new()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            PromoCodes = customer.PromoCodes?.Select(pc => new PromoCodeShortResponse
            {
                Id = pc.Id,
                Code = pc.Code,
                ServiceInfo = pc.ServiceInfo,
                BeginDate = pc.BeginDate.ToString(),
                EndDate = pc.EndDate.ToString(),
                PartnerName = pc.PartnerName
            }).ToList(),
            CustomerPreferences = customer.CustomerPreferences?.Select(pc => new CustomerPreference
            {
                CustomerId = pc.CustomerId,
                PreferenceId = pc.PreferenceId,
                Id = pc.Id,
            }).ToList()
        };

        public static List<CustomerShortResponse> ToShort(IEnumerable<Customer> customers)
        {
            return customers.Select(e => ToShort(e)).ToList();
        }

        public static CustomerShortResponse ToShort(Customer customer) => new()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email
        };
    }
}

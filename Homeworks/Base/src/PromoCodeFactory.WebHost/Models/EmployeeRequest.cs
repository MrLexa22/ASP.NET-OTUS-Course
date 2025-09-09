using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models
{
    public class EmployeeRequest
    {
        public Guid? Id { get; set; } = null;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<Guid>? Roles { get; set; } = null;
        public int AppliedPromocodesCount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class CustomerPreference : BaseEntity
    {
        public Guid CustomerId { get; set; }

        public Customer Customer { get; set; }

        public Guid PreferenceId { get; set; }

        public Preference Preference { get; set; }
    }
}

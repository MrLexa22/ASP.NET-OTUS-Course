using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Preference
        : BaseEntity
    {
        public string Name { get; set; }

        public List<CustomerPreference> CustomerPreferences { get; set; }
        public List<PromoCode> PromoCodes { get; set; }
    }
}
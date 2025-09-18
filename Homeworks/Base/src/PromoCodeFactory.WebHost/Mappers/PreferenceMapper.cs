using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.WebHost.Mappers
{
    public class PreferenceMapper
    {
        public static List<PrefernceResponse> ToResponse(IEnumerable<Preference> preferences)
        {
            return preferences.Select(e => ToResponse(e)).ToList();
        }

        public static PrefernceResponse ToResponse(Preference preference) => new()
        {
            Id = preference.Id,
            Name = preference.Name
        };
    }
}

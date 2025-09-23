using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.WebHost.Mappers
{
    public class PromocodeMapper
    {
        public static List<PromoCodeShortResponse> ToShort(IEnumerable<PromoCode> promoCodes)
        {
            return promoCodes.Select(e => ToShort(e)).ToList();
        }

        public static PromoCodeShortResponse ToShort(PromoCode promoCode) => new()
        {
            Id = promoCode.Id,
            Code = promoCode.Code,
            ServiceInfo = promoCode.ServiceInfo,
            BeginDate = promoCode.BeginDate.ToUniversalTime().ToString("O"),
            EndDate = promoCode.EndDate.ToUniversalTime().ToString("O"),
            PartnerName = promoCode.PartnerName
        };
    }
}

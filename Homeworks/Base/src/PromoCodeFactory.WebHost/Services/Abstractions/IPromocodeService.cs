using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Services.Abstractions
{
    public interface IPromocodeService
    {
        Task<List<PromoCodeShortResponse>> GetListPromocodesShortResponseAsync();
        Task<bool> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request);
    }
}

using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Services.Abstractions
{
    public interface IPreferenceService
    {
        Task<List<PrefernceResponse>> GetPreferences();
    }
}

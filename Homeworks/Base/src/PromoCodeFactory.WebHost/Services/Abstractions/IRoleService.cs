using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Services.Abstractions
{
    public interface IRoleService
    {
        Task<List<RoleItemResponse>> GetRolesAsync();
    }
}

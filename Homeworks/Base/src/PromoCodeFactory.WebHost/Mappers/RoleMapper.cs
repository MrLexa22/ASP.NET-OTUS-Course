using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.WebHost.Mappers
{
    public static class RoleMapper
    {
        public static List<RoleItemResponse> ToRoleItemResponse(IEnumerable<Role> roles)
        {
            return roles.Select(e => ToRoleItemResponse(e)).ToList();
        }

        public static RoleItemResponse ToRoleItemResponse(Role x) => new()
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description
        };
    }
}

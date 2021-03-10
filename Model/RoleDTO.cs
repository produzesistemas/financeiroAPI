
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Model
{
    public class RoleDTO : BaseEntity
    {
        public List<IdentityRole> modulos;
        public RoleDTO()
        {
            modulos = new List<IdentityRole>();
        }
    }
}

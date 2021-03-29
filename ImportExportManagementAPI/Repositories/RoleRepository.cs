using ImportExportManagement_API.Models;
using ImportExportManagement_API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Repositories
{
    public class RoleRepository : BaseRepository<Role>
    {
        public Role GetRoleByName(String roleName)
        {
            return (Role)_dbSet.Where(r => r.RoleName.Equals(roleName));
        }
    }
}



using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Models;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



/**
* @author Loi Nguyen
*
* @date - 3/17/2021 12:53:56 PM 
*/

namespace ImportExportManagementAPI.Repositories
{
    public class SystemConfigRepository : BaseRepository<SystemConfig>
    {
        public String GetAutoSchedule()
        {
            String ss = AttributeKey.AutoSchedule.ToString();
            return _dbSet.Where(s => s.AttributeKey == AttributeKey.AutoSchedule.ToString()).Select(s => s.AttributeValue).ToString();
        }

    }
}



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
            return _dbSet.Where(s => s.AttributeKey == AttributeKey.AutoSchedule.ToString()).Select(s => s.AttributeValue).SingleOrDefault();
        }

        public String GetStorageCapacity()
        {
            return _dbSet.Where(s => s.AttributeKey == AttributeKey.StorageCapacity.ToString()).Select(s => s.AttributeValue).SingleOrDefault();
        }
        public String GetMaximumSlot()
        {
            return _dbSet.Where(s => s.AttributeKey == AttributeKey.MaximumSlot.ToString()).Select(s => s.AttributeValue).SingleOrDefault();
        }
        public String GetMaximumCanceledSchechule()
        {
            return _dbSet.Where(s => s.AttributeKey == AttributeKey.MaximumCanceledSchechule.ToString()).Select(s => s.AttributeValue).SingleOrDefault();
        }

        public String GetTimeBetweenSlot()
        {
            return _dbSet.Where(s => s.AttributeKey == AttributeKey.TimeBetweenSlot.ToString()).Select(s => s.AttributeValue).SingleOrDefault();
        }
        public async Task<Boolean> UpdateAutoSchedule(String time)
        {
            TimeSpan newTime;
            if (TimeSpan.TryParse(time, out newTime))
            {
                SystemConfig autoSchedule = await _dbSet.FindAsync(AttributeKey.AutoSchedule.ToString());
                if (autoSchedule != null)
                {
                    autoSchedule.AttributeValue = time;
                    _dbContext.Entry(autoSchedule).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await SaveAsync();
                    return true;
                }
            }
            return false;
        }
    }
}

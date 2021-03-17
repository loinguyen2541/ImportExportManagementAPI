using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

/**
* @author Loi Nguyen
*
* @date - 3/11/2021 12:20:01 PM 
*/

namespace ImportExportManagementAPI.Repositories
{
    public class TimeTemplateRepository : BaseRepository<TimeTemplate>
    {
        public async void ResetSchedule(float capacity)
        {

            TimeTemplate timeTemplate = await _dbSet.Include(t => t.TimeTemplateItems)
                .Where(p => p.TimeTemplateStatus == TimeTemplateStatus.Applied).SingleAsync();
            foreach (var item in timeTemplate.TimeTemplateItems)
            {
                item.Capacity = capacity;
            }

            _dbContext.Entry(timeTemplate).State = EntityState.Modified;

            await SaveAsync();
        }

        public async Task<TimeTemplate> GetCurrentTimeTemplateAsync(int partnerId)
        {
            TimeTemplate timeTemplate = await _dbSet.
                Where(t => t.TimeTemplateStatus == TimeTemplateStatus.Applied)
                .Include(t => t.TimeTemplateItems).ThenInclude(t => t.Schedules.Where(p => p.PartnerId == partnerId && p.IsCanceled == false))
                .SingleOrDefaultAsync();
            return timeTemplate;
        }
    }
}

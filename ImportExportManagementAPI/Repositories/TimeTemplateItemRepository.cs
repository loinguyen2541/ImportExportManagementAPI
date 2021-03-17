using ImportExportManagement_API.Models;
using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/**
* @author Loi Nguyen
*
* @date - 3/11/2021 3:51:20 PM 
*/

namespace ImportExportManagementAPI.Repositories
{
    public class TimeTemplateItemRepository : BaseRepository<TimeTemplateItem>
    {
        public bool CheckCapacity(float registeredWeight, int id)
        {
            float capacity = _dbSet.Find(id).Capacity;
            if (capacity > registeredWeight)
            {
                return true;
            }
            return false;
        }

        public async void UpdateSchedule(TransactionType type, float registeredWeight, int id)
        {
            TimeTemplateItem timeTemplateItem = await _dbSet.FindAsync(id);
            float targetItemCapacity = 0;
            List<TimeTemplateItem> timeTemplateItems = await _dbSet
                .Where(i => i.TimeTemplateId == timeTemplateItem.TimeTemplateId)
                .OrderBy(p => p.ScheduleTime).ToListAsync();
            if (type == TransactionType.Export)
            {
                targetItemCapacity = timeTemplateItem.Capacity + registeredWeight;
                UpdateCapacityExport(timeTemplateItems, timeTemplateItem.ScheduleTime, targetItemCapacity, registeredWeight);
            }
            else
            {
                targetItemCapacity = timeTemplateItem.Capacity - registeredWeight;
                UpdateCapacityImport(timeTemplateItems, timeTemplateItem.ScheduleTime, targetItemCapacity, registeredWeight);
            }
            await _dbContext.SaveChangesAsync();
        }

        public void UpdateCapacityImport(List<TimeTemplateItem> timeTemplateItems, TimeSpan targetTime, float targetCapacity, float registeredWeight)
        {
            foreach (var item in timeTemplateItems)
            {
                if (item.ScheduleTime < targetTime)
                {
                    if (item.Capacity > targetCapacity)
                    {
                        item.Capacity = targetCapacity;
                    }
                }
                else if (item.ScheduleTime == targetTime)
                {
                    item.Capacity = targetCapacity;
                }
                else
                {
                    item.Capacity -= registeredWeight;
                }
                _dbContext.Entry(item).State = EntityState.Modified;
            }
        }
        public void UpdateCapacityExport(List<TimeTemplateItem> timeTemplateItems, TimeSpan targetTime, float targetCapacity, float registeredWeight)
        {
            foreach (var item in timeTemplateItems)
            {
                if (item.ScheduleTime == targetTime)
                {
                    item.Capacity = targetCapacity;
                }
                else if (item.ScheduleTime > targetTime)
                {
                    item.Capacity += registeredWeight;
                }
                _dbContext.Entry(item).State = EntityState.Modified;
            }
        }

        public async Task<List<TimeTemplateItem>> GetAppliedItem()
        {
            return await _dbSet.Include(i => i.Schedules.Where(s => s.IsCanceled == false)).Where(i => i.TimeTemplate.TimeTemplateStatus == TimeTemplateStatus.Applied).ToListAsync();
        }
    }
}

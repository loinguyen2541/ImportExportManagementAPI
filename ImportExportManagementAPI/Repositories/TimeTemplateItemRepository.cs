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
            float capacity = _dbSet.Find(id).Inventory;
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
            List<TimeTemplateItem> timeTemplateItems = null;
            if (timeTemplateItem != null)
            {
                timeTemplateItems = await _dbSet
                .Where(i => i.TimeTemplateId == timeTemplateItem.TimeTemplateId)
                .OrderBy(p => p.ScheduleTime).ToListAsync();
            }
            if (timeTemplateItems != null || timeTemplateItems.Count > 0)
            {
                if (type == TransactionType.Export)
                {
                    targetItemCapacity = timeTemplateItem.Inventory - registeredWeight;
                    UpdateCapacityExport(timeTemplateItems, timeTemplateItem.ScheduleTime, targetItemCapacity, registeredWeight);
                }
                else
                {
                    targetItemCapacity = timeTemplateItem.Inventory + registeredWeight;
                    UpdateCapacityImport(timeTemplateItems, timeTemplateItem.ScheduleTime, targetItemCapacity, registeredWeight);
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        public void UpdateCapacityImport(List<TimeTemplateItem> timeTemplateItems, TimeSpan targetTime, float targetInventory, float registeredWeight)
        {
            foreach (var item in timeTemplateItems)
            {
                if (item.ScheduleTime < targetTime)
                {
                    if (item.Inventory > targetInventory)
                    {
                        item.Inventory = targetInventory;
                    }
                }
                else if (item.ScheduleTime == targetTime)
                {
                    item.Inventory = targetInventory;
                }
                else
                {
                    item.Inventory += registeredWeight;
                }
                _dbContext.Entry(item).State = EntityState.Modified;
            }
        }
        public void UpdateCapacityExport(List<TimeTemplateItem> timeTemplateItems, TimeSpan targetTime, float targetInventory, float registeredWeight)
        {
            foreach (var item in timeTemplateItems)
            {
                if (item.ScheduleTime == targetTime)
                {
                    item.Inventory = targetInventory;
                }
                else if (item.ScheduleTime > targetTime)
                {
                    item.Inventory -= registeredWeight;
                }
                _dbContext.Entry(item).State = EntityState.Modified;
            }
        }

        public async Task<List<TimeTemplateItem>> GetAppliedItem()
        {
            return await _dbSet.Include(i => i.Schedules.Where(s => s.IsCanceled == false)).Where(i => i.TimeTemplate.TimeTemplateStatus == TimeTemplateStatus.Applied).ToListAsync();
        }

        public async Task<bool> CancelSchedule(Schedule schedule)
        {
            bool checkCancel = false;
            TimeTemplateItem timeTemplateItem = await _dbSet.FindAsync(schedule.TimeTemplateItemId);
            List<TimeTemplateItem> timeTemplateItems = new List<TimeTemplateItem>();
            if (timeTemplateItem != null)
            {
                if (CheckValidTime(timeTemplateItem))
                {
                    timeTemplateItems = await _dbSet
                .Where(i => i.TimeTemplateId == timeTemplateItem.TimeTemplateId)
                .OrderBy(p => p.ScheduleTime).ToListAsync();
                    RedefineByTimeClick(timeTemplateItems);
                }
            }
            if (timeTemplateItems != null && timeTemplateItems.Count > 0)
            {
                if (schedule.TransactionType.Equals(TransactionType.Import))
                {
                    CancelImport(timeTemplateItems, timeTemplateItem.ScheduleTime, schedule.RegisteredWeight);
                }
                else if (schedule.TransactionType.Equals(TransactionType.Export))
                {
                    CancelExport(timeTemplateItems, timeTemplateItem.ScheduleTime, schedule.RegisteredWeight);
                }
                try
                {
                    await _dbContext.SaveChangesAsync();
                    checkCancel = true;
                }
                catch
                {
                    checkCancel = false;
                }
            }
            
            return checkCancel;
        }
        public void CancelImport(List<TimeTemplateItem> timeTemplateItems, TimeSpan targetTime, float registeredWeight)
        {
            foreach (var item in timeTemplateItems)
            {
                if (item.ScheduleTime >= targetTime)
                {
                    item.Inventory = item.Inventory + registeredWeight;
                }
                _dbContext.Entry(item).State = EntityState.Modified;
            }
        }
        public void CancelExport(List<TimeTemplateItem> timeTemplateItems, TimeSpan targetTime, float registeredWeight)
        {
            foreach (var item in timeTemplateItems)
            {
                if (item.ScheduleTime >= targetTime)
                {
                    //ko xuất kho, tồn kho tăng lên
                    item.Inventory = item.Inventory + registeredWeight;
                }
                _dbContext.Entry(item).State = EntityState.Modified;
            }

        }

        public Boolean CheckValidTime(TimeTemplateItem scheduleTime)
        {
            bool check = false;
            TimeSpan current = DateTime.Now.TimeOfDay;
            TimeSpan scheduleDate = scheduleTime.ScheduleTime;
            TimeSpan result = current.Subtract(scheduleDate);
            if(current >= scheduleDate)
            {

            }
            else
            {
                if ((int)result.TotalMinutes <= 30)
                {
                    check = true;
                }
            }
            return check;
        }

        public List<TimeTemplateItem> RedefineByTimeClick(List<TimeTemplateItem> listTime)
        {
            List<TimeTemplateItem> listTemp = new List<TimeTemplateItem>();
            TimeSpan current = DateTime.Now.TimeOfDay;
            foreach (var item in listTime)
            {
                if (current < item.ScheduleTime)
                {
                    listTemp.Add(item);
                }
            }
            return listTemp;
        }
    }
}

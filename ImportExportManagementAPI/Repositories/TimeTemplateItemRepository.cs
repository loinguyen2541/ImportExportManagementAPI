using ImportExportManagement_API.Models;
using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Controllers;
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

        public bool CheckInventory(float registeredWeight, int id, TransactionType type, float storageCapacity)
        {
            //check tồn kho đã đạt giới hạn chưa
            GoodsRepository _goodsRepo = new GoodsRepository();
            Goods goods = _dbContext.Goods.First();
            if (goods != null)
            {
                float inventory = _dbSet.Find(id).Inventory;
                if (type == TransactionType.Import)
                {
                    if (goods.QuantityOfInventory == 2000)
                    {
                        return false;
                    }
                    else
                    {
                        float availableStorageCapacity = storageCapacity - inventory;
                        if (availableStorageCapacity >= registeredWeight)
                        {
                            return true;
                        }
                    }
                }
                else if (type == TransactionType.Export)
                {
                    if (goods.QuantityOfInventory == 0)
                    {
                        return false;
                    }
                    else
                    {
                        if (inventory >= registeredWeight)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            return false;
        }

        public bool CheckCapacity(float registeredWeight, int id)
        {
            float capacity = _dbSet.Find(id).Inventory;
            if (capacity > registeredWeight)
            {
                return true;
            }
            return false;
        }

        public void UpdateCurrent(TransactionType type, float registeredWeight, int id)
        {
            TimeTemplateItem timeTemplateItem = _dbSet.Find(id);
            float targetItemCapacity = 0;
            List<TimeTemplateItem> timeTemplateItems = null;
            if (timeTemplateItem != null)
            {
                timeTemplateItems = _dbSet
                .Where(i => i.TimeTemplateId == timeTemplateItem.TimeTemplateId)
                .OrderBy(p => p.ScheduleTime).ToList();
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
                _dbContext.SaveChanges();
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
                else if (item.ScheduleTime < targetTime && item.Inventory >= targetInventory)
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
        public void UpdateCapacityImport(List<TimeTemplateItem> timeTemplateItems, TimeSpan targetTime, float targetInventory, float registeredWeight)
        {
            foreach (var item in timeTemplateItems)
            {
                if (item.ScheduleTime == targetTime)
                {
                    item.Inventory = targetInventory;
                }
                else if (item.ScheduleTime > targetTime)
                {
                    item.Inventory += registeredWeight;
                }
                _dbContext.Entry(item).State = EntityState.Modified;
            }
        }

        public async Task<List<TimeTemplateItem>> GetAppliedItem()
        {
            return await _dbSet.Include(i => i.Schedules.Where(s => s.ScheduleStatus == ScheduleStatus.Approved)).Where(i => i.TimeTemplate.TimeTemplateStatus == TimeTemplateStatus.Applied).ToListAsync();
        }

        public async Task<Schedule> CancelSchedule(Schedule schedule, String username)
        {
            TimeTemplateItem timeTemplateItem = await _dbSet.FindAsync(schedule.TimeTemplateItemId);
            List<TimeTemplateItem> timeTemplateItems = new List<TimeTemplateItem>();
            if (timeTemplateItem != null)
            {
                if (CheckValidTime(timeTemplateItem))
                {
                    timeTemplateItems = await _dbSet
                .Where(i => i.TimeTemplateId == timeTemplateItem.TimeTemplateId)
                .OrderBy(p => p.ScheduleTime).ToListAsync();
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
                schedule.ScheduleStatus = ScheduleStatus.Cancel;
                schedule.UpdatedBy = username;
                try
                {
                    await _dbContext.SaveChangesAsync();
                    return schedule;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
        public void CancelImport(List<TimeTemplateItem> timeTemplateItems, TimeSpan targetTime, float registeredWeight)
        {
            foreach (var item in timeTemplateItems)
            {
                if (item.ScheduleTime >= targetTime)
                {
                    item.Inventory = item.Inventory - registeredWeight;
                }
                _dbContext.Entry(item).State = EntityState.Modified;
            }
        }
        public void CancelExport(List<TimeTemplateItem> timeTemplateItems, TimeSpan targetTime, float registeredWeight)
        {
            foreach (var item in timeTemplateItems)
            {
                //ko xuất kho, tồn kho tăng lên
                item.Inventory = item.Inventory + registeredWeight;
                _dbContext.Entry(item).State = EntityState.Modified;
            }

        }

        public async Task<Schedule> ChangeSchedule(Schedule updateSchedule, Schedule existedSchedule)
        {
            Schedule cancelSchedule = await CancelSchedule(existedSchedule, "Update action");
            if (cancelSchedule != null)
            {
                if (CheckCapacity(updateSchedule.RegisteredWeight, updateSchedule.TimeTemplateItemId))
                {
                    UpdateCurrent(updateSchedule.TransactionType, updateSchedule.RegisteredWeight, updateSchedule.TimeTemplateItemId);
                    updateSchedule.ScheduleStatus = ScheduleStatus.Cancel;
                    updateSchedule.RegisteredWeight = updateSchedule.RegisteredWeight;
                    return updateSchedule;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public Boolean CheckValidTime(TimeTemplateItem scheduleTime)
        {
            bool check = false;
            TimeSpan current = DateTime.Now.TimeOfDay;
            TimeSpan scheduleDate = scheduleTime.ScheduleTime;
            TimeSpan result = current.Subtract(scheduleDate);
            if (current >= scheduleDate)
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

        //public async Task<string> UpdateScheduleSameTypeAsync(Schedule beforeSchedule, Schedule afterSchedule)
        //{
        //    String message = "";
        //    TimeTemplateItem timeTemplateItemBefore = await _dbSet.FindAsync(beforeSchedule.TimeTemplateItemId);
        //    TimeTemplateItem timeTemplateItemAfter = await _dbSet.FindAsync(afterSchedule.TimeTemplateItemId);
        //    List<TimeTemplateItem> timeTemplateItems = new List<TimeTemplateItem>();
        //    float targetWeight = 0;
        //    if (beforeSchedule.TimeTemplateItemId == afterSchedule.TimeTemplateItemId)
        //    {
        //        //cung gio - xuat kho - giam het capacity
        //        if (afterSchedule.TransactionType.Equals(TransactionType.Export))
        //        {
        //            timeTemplateItems = await _dbSet.OrderBy(p => p.ScheduleTime).ToListAsync();
        //            targetWeight = beforeSchedule.RegisteredWeight - afterSchedule.RegisteredWeight;
        //            UpdateExport(timeTemplateItems, targetWeight,"sametype");
        //        }
        //        //cung gio - nhap kho - tang capacity
        //        if (afterSchedule.TransactionType.Equals(TransactionType.Import))
        //        {
        //            timeTemplateItems = await _dbSet.Where(i => i.TimeTemplateId == timeTemplateItemAfter.TimeTemplateId).OrderBy(p => p.ScheduleTime).ToListAsync();
        //            targetWeight = beforeSchedule.RegisteredWeight + afterSchedule.RegisteredWeight;

        //        }
        //    }
        //    else
        //    {
        //        //khac gio

        //    }
        //    try
        //    {
        //        await _dbContext.SaveChangesAsync();
        //    }
        //    catch
        //    {
        //    }
        //    return message;
        //}
    }
}

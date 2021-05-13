using ImportExportManagement_API.Models;
using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Controllers;
using ImportExportManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
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

            TimeTemplateItem timeTemplateItem = _dbSet.Where(i => i.TimeTemplateItemId == id).SingleOrDefault();

            _dbContext.Entry(timeTemplateItem).Reload();

            if (type == TransactionType.Import)
            {
                if ((storageCapacity - timeTemplateItem.Inventory) < registeredWeight)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (type == TransactionType.Export)
            {
                if (timeTemplateItem.Inventory >= registeredWeight)
                {
                    return true;
                }
                else
                {
                    return false;
                }
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

        public TransactionType DefineTransactionType(int partnerId)
        {
            TransactionType type = TransactionType.Import;
            Partner partner = _dbContext.Partner.Find(partnerId);
            if (partner != null)
            {
                if (partner.PartnerTypeId == 1)
                {
                    return TransactionType.Export;
                }
                else if (partner.PartnerTypeId == 2)
                {
                    return TransactionType.Import;
                }
            }
            return type;
        }

        public void UpdateCurrent(TransactionType type, float registeredWeight, int timeItemId)
        {
            TimeTemplateItem timeTemplateItem = _dbSet.Find(timeItemId);
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
            ScheduleRepository scheduleRepository = new ScheduleRepository();
            GoodsRepository goodsRepository = new GoodsRepository();
            List<TimeTemplateItem> timeTemplateItems = await _dbSet.Where(i => i.Status == TimeTemplateStatus.Applied)
                .Include(i => i.Schedules.Where(s => s.ScheduleStatus == ScheduleStatus.Approved))
                .Where(i => i.TimeTemplate.TimeTemplateStatus == TimeTemplateStatus.Applied).OrderBy(o => o.ScheduleTime).ToListAsync();
       
            return await Task.Run(() => timeTemplateItems);
        }

        public async Task<List<TimeTemplateItem>> GetAppliedItemByScheduleType(string scheduleType)
        {
            ScheduleRepository scheduleRepository = new ScheduleRepository();
            GoodsRepository goodsRepository = new GoodsRepository();
            float totalImportExpected = 0;
            float totalExportExpected = 0;
            List<TimeTemplateItem> timeTemplateItems = await _dbSet
                .Where(i => i.Status == TimeTemplateStatus.Applied)
               .Include(i => i.TimeTemplate)
               .Where(i => i.TimeTemplate.TimeTemplateStatus == TimeTemplateStatus.Applied 
               && i.TimeTemplate.ApplyingDate.Date == DateTime.Now.Date
               )
                .OrderBy(o => o.ScheduleTime).ToListAsync();
            float goodsCapacity = goodsRepository.GetGoodCapacity();
            List<Schedule> schedules = scheduleRepository.GetAllAsyncToday();
            if (scheduleType == "Import")
            {
                foreach (var timeTemplateItem in timeTemplateItems)
                {
                    timeTemplateItem.Inventory = 0;
                    timeTemplateItem.Inventory = totalImportExpected + goodsCapacity - totalExportExpected;
                    foreach (var schedule in schedules)
                    {

                        if (schedule.ScheduleDate.ToString("HH:mm:ss").Contains(timeTemplateItem.ScheduleTime.ToString()))
                        {
                            if (schedule.TransactionType == TransactionType.Import)
                            {
                                totalImportExpected += schedule.RegisteredWeight;
                            }
                            else
                            {
                                totalExportExpected += schedule.RegisteredWeight;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var timeTemplateItem in timeTemplateItems)
                {
                    timeTemplateItem.Inventory = 0;
                    timeTemplateItem.Inventory = totalImportExpected + goodsCapacity - totalExportExpected;
                    foreach (var schedule in schedules)
                    {

                        if (schedule.ScheduleDate.ToString("HH:mm:ss").Contains(timeTemplateItem.ScheduleTime.ToString()))
                        {
                            if (schedule.TransactionType == TransactionType.Import)
                            {
                                totalImportExpected += schedule.RegisteredWeight;
                            }
                            else
                            {
                                foreach (var item in timeTemplateItems)
                                {
                                    if (item.ScheduleTime.ToString().CompareTo(schedule.ScheduleDate.ToString("HH:mm:ss")) <= 0)
                                    {
                                        item.Inventory -= schedule.RegisteredWeight;
                                    }
                                }
                                totalExportExpected += schedule.RegisteredWeight;
                            }
                        }
                    }
                }

            }


            return await Task.Run(() => timeTemplateItems); ;
        }
        public async Task<Schedule> CancelSchedule(Schedule schedule, String username)
        {
            TimeTemplateItem timeTemplateItem = await _dbSet.FindAsync(schedule.TimeTemplateItemId);
            List<TimeTemplateItem> timeTemplateItems = new List<TimeTemplateItem>();
            if (timeTemplateItem != null)
            {
                if (CheckValidTime(schedule.TimeTemplateItemId))
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

        public async Task<String> ChangeSchedule(Schedule updateSchedule, Schedule existedSchedule)
        {
            if (CheckCapacity(updateSchedule.RegisteredWeight, updateSchedule.TimeTemplateItemId))
            {
                Schedule cancelSchedule = await CancelSchedule(existedSchedule, "Update action");
                if (cancelSchedule != null)
                {
                    UpdateCurrent(updateSchedule.TransactionType, updateSchedule.RegisteredWeight, updateSchedule.TimeTemplateItemId);
                    return "";
                }
                else
                {
                    return "Change schedule failed";
                }
            }
            return "Inventory is not available";
        }

        public Boolean CheckValidTime(int timeItemId)
        {
            TimeTemplateItem scheduleTime = _dbSet.Find(timeItemId);
            TimeSpan current = DateTime.Now.TimeOfDay;
            TimeSpan timetemplateitemTimespan = scheduleTime.ScheduleTime;
            TimeSpan result = current.Subtract(timetemplateitemTimespan);
            if ((int)result.TotalMinutes <= 30)
            {
                return true;
            }
            else
            {
                return false;
            }
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

        public List<TimeTemplateItem> GetPendingTimeTemplateItems()
        {
            return _dbSet.Where(t => t.Status == TimeTemplateStatus.Pending).ToList();
        }

        public List<TimeTemplateItem> GetAppliedTimeTemplateItems()
        {
            return _dbSet.Where(t => t.Status == TimeTemplateStatus.Applied).ToList();
        }

        public void ChangePendingToAppliedTimeTemplateItem()
        {
            List<TimeTemplateItem> pendingTimeTemplateItems = GetPendingTimeTemplateItems();
            if (pendingTimeTemplateItems != null && pendingTimeTemplateItems.Count > 0)
            {
                List<TimeTemplateItem> appliedTimeTemplateItems = GetAppliedTimeTemplateItems();
                foreach (var item in appliedTimeTemplateItems)
                {
                    item.Status = TimeTemplateStatus.Disabled;
                    _dbContext.Entry(item).State = EntityState.Modified;
                }
                foreach (var item in pendingTimeTemplateItems)
                {
                    item.Status = TimeTemplateStatus.Applied;
                    _dbContext.Entry(item).State = EntityState.Modified;
                }
                _dbContext.SaveChanges();
            }
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

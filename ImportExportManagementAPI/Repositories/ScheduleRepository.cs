using ImportExportManagement_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ImportExportManagementAPI.Models;

/**
* @author Loi Nguyen
*
* @date - 2/5/2021 10:50:39 AM 
*/

namespace ImportExportManagement_API.Repositories
{
    public class ScheduleRepository : BaseRepository<Schedule>
    {
        public async ValueTask<Pagination<Schedule>> GetAllAsync(PaginationParam paging, ScheduleFilterParam filter)
        {
            Pagination<Schedule> schedules = new Pagination<Schedule>();
            IQueryable<Schedule> rawData = null;
            rawData = _dbSet.Include(s => s.Partner);
            schedules = await DoFilter(paging, filter, rawData);
            return schedules;
        }

        public async ValueTask<List<Schedule>> GetHistory(string searchDate)
        {
            List<Schedule> schedules = new List<Schedule>();
            IQueryable<Schedule> rawData = null;
            rawData = _dbSet.Include(t => t.TimeTemplateItem);
            schedules = await DoFilterHistory(searchDate, rawData);
            foreach (var item in schedules)
            {
                //date of schedule
                item.ScheduleDate = ChangeTime(item.ScheduleDate, item.TimeTemplateItem.ScheduleTime.Hours, item.TimeTemplateItem.ScheduleTime.Minutes, item.TimeTemplateItem.ScheduleTime.Seconds);
            }
            return schedules;
        }
        public DateTime ChangeTime(DateTime dateTime, int hours, int minutes, int seconds)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                hours,
                minutes,
                seconds,
                dateTime.Kind);
        }

        private async Task<List<Schedule>> DoFilterHistory(String searchDate, IQueryable<Schedule> queryable)
        {
            if (DateTime.TryParse(searchDate, out DateTime date))
            {
                DateTime start = DateTime.Parse(searchDate);
                DateTime end = DateTime.Parse(searchDate).AddDays(1);
                queryable = queryable.Where(s => start <= s.ScheduleDate && s.ScheduleDate <= end);
            }
            queryable = queryable.Where(s => !s.UpdatedBy.Contains("update action"));
            return await queryable.OrderBy(s => s.TimeTemplateItem.ScheduleTime).ToListAsync();
        }
        public async Task<List<Schedule>> GetByPartnerId(int partnerId)
        {
            List<Schedule> schedules = await _dbSet
                .Where(s => s.PartnerId == partnerId && s.IsCanceled == false).OrderBy(s => s.ScheduleDate).ToListAsync();
            return schedules;
        }

        private async Task<Pagination<Schedule>> DoFilter(PaginationParam paging, ScheduleFilterParam filter, IQueryable<Schedule> queryable)
        {
            if (filter.PartnerName != null && filter.PartnerName.Length > 0)
            {
                queryable = queryable.Where(p => p.Partner.DisplayName.Contains(filter.PartnerName));
            }
            if (DateTime.TryParse(filter.ScheduleDate, out DateTime date))
            {
                DateTime scheduleDate = DateTime.Parse(filter.ScheduleDate);
                queryable = queryable.Where(p => p.ScheduleDate.Date == scheduleDate.Date);
            }
            if (Enum.TryParse(filter.TransactionType, out TransactionType transactionType))
            {
                TransactionType type = (TransactionType)Enum.Parse(typeof(TransactionType), filter.TransactionType);
                queryable = queryable.Where(p => p.TransactionType == type);
            }

            if (paging.Page < 1)
            {
                paging.Page = 1;
            }
            if (paging.Size < 1)
            {
                paging.Size = 1;
            }

            int count = queryable.Count();

            if (((paging.Page - 1) * paging.Size) > count)
            {
                paging.Page = 1;
            }

            queryable = queryable.Skip((paging.Page - 1) * paging.Size).Take(paging.Size);

            Pagination<Schedule> pagination = new Pagination<Schedule>();
            pagination.Page = paging.Page;
            pagination.Size = paging.Size;
            double totalPage = (count * 1.0) / (pagination.Size * 1.0);
            pagination.TotalPage = (int)Math.Ceiling(totalPage);
            pagination.Data = await queryable.ToListAsync();

            return pagination;
        }

        /// <summary>
        /// Change all approved schedules to cancel
        /// </summary>
        public async void DisableAll()
        {
            List<Schedule> schedules = await _dbSet.Where(p => p.ScheduleStatus == ScheduleStatus.Approved).ToListAsync();
            foreach (var item in schedules)
            {
                item.ScheduleStatus = ScheduleStatus.Cancel;
                item.UpdatedBy = SystemName.System.ToString();
                _dbContext.Entry(item).State = EntityState.Modified;
            }
            await SaveAsync();
        }



        public int Count()
        {
            return _dbSet.Count();
        }

        public new void Delete(Schedule schedule)
        {
            schedule.IsCanceled = true;
            Update(schedule);
        }
        public new void Delete(object id)
        {
            Schedule schedule = _dbSet.Find(id);
            schedule.IsCanceled = true;
            Update(schedule);
        }

        public bool Exist(int id)
        {
            return _dbSet.Any(e => e.ScheduleId == id);
        }

        public Boolean TryToUpdate(Schedule schedule)
        {
            Schedule currentSchedule = _dbSet.Where(s => s.IsCanceled == false && s.TransactionType == schedule.TransactionType && s.TimeTemplateItemId == schedule.TimeTemplateItemId).SingleOrDefault();
            if (currentSchedule != null)
            {
                currentSchedule.RegisteredWeight += schedule.RegisteredWeight;
                _dbContext.Entry(currentSchedule).State = EntityState.Modified;
                return true;
            }
            return false;
        }

        public async Task<List<Schedule>> GetTop10Schedule()
        {
            IQueryable<Schedule> rawData = null;
            DateTime now = DateTime.Today;
            DateTime yesterday = now.AddDays(-1);
            DateTime tomorrow = now.AddDays(1);
            rawData = _dbSet.Include(s => s.Partner).Where(s => s.CreatedDate > yesterday && s.CreatedDate < tomorrow && s.ScheduleStatus == ScheduleStatus.Approved).OrderByDescending(o=>o.ScheduleId);
            return await rawData.Take(10).ToListAsync();
        }

    }

    enum SystemName
    {
        System
    }
}
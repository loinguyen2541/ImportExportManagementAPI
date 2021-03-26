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
        public async Task<Schedule> GetScheduleById(int id)
        {
            var schedule = await _dbSet.Include(s => s.Goods).Where(s => s.ScheduleId == id).SingleOrDefaultAsync();

            if (schedule == null)
            {
                return null;
            }

            return schedule;
        }
        public async ValueTask<Pagination<Schedule>> GetAllAsync(PaginationParam paging, ScheduleFilterParam filter)
        {
            Pagination<Schedule> schedules = new Pagination<Schedule>();
            IQueryable<Schedule> rawData = null;
            rawData = _dbSet.Include(s => s.Partner);
            schedules = await DoFilter(paging, filter, rawData);
            return schedules;
        }

        public async ValueTask<List<Schedule>> GetHistory(string searchDate, string type)
        {
            List<Schedule> schedules = new List<Schedule>();
            IQueryable<Schedule> rawData = null;
            rawData = _dbSet.Include(t => t.TimeTemplateItem);
            schedules = await DoFilterHistory(searchDate, rawData, type);
            foreach (var item in schedules)
            {
                //date of schedule
                item.ScheduleDate = ChangeTime(item.ScheduleDate, item.TimeTemplateItem.ScheduleTime.Hours, item.TimeTemplateItem.ScheduleTime.Minutes, item.TimeTemplateItem.ScheduleTime.Seconds);
            }
            return schedules;
        }
        public async ValueTask<List<Schedule>> GetListScheduleByRequire(string fromDate, string toDate, int caseSearch)
        {
            List<Schedule> schedules = new List<Schedule>();
            IQueryable<Schedule> rawData = null;
            rawData = _dbSet.Include(t => t.TimeTemplateItem);
            schedules = await DoFilterReport(rawData, fromDate, toDate, caseSearch);
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

        private async Task<List<Schedule>> DoFilterReport(IQueryable<Schedule> queryable, String fromDate, String toDate, int caseSearch)
        {
            if (DateTime.TryParse(fromDate, out DateTime date) && DateTime.TryParse(toDate, out DateTime date2))
            {
                DateTime start = DateTime.Parse(fromDate);
                DateTime end = DateTime.Parse(toDate);
                queryable = queryable.Where(s => start <= s.ScheduleDate && s.ScheduleDate <= end);
            }
            if(queryable != null)
            {
                switch (caseSearch)
                {
                    case 1:
                        //đặt lịch mà giao => check by realweight
                        queryable = queryable.Where(s => s.IsCanceled == false && s.ScheduleStatus == ScheduleStatus.Success && s.ActualWeight != null);
                        break;
                    case 2:
                        //đặt lịch mà không giao => bị hủy bởi hệ thống => check by iscancel và update by hệ thống
                        queryable = queryable.Where(s => s.IsCanceled == true && s.ScheduleStatus == ScheduleStatus.Cancel && s.UpdatedBy.Equals("system"));
                        break;
                    default:
                        break;
                }
            }
            return await queryable.OrderBy(s => s.TimeTemplateItem.ScheduleTime).ToListAsync();
        }

        public int GetTotalByType(int type)
        {
            int count = 0;
            TransactionType typeTrans = (TransactionType)type;
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now.AddDays(1);
            IQueryable<Schedule> rawData = _dbSet.Where(s => start <= s.ScheduleDate && s.ScheduleDate <= end && s.TransactionType.Equals(typeTrans));
            count = rawData.Count();

            return count;
        }

        public async Task<Pagination<Schedule>> DoFilterSearchPartner(ScheduleFilterParam filter, PaginationParam paging)
        {
            IQueryable<Schedule> queryable = _dbSet.Include(s => s.Partner);
            if(filter == null)
            {
                DateTime start = DateTime.Now.AddDays(-1);
                DateTime end = DateTime.Now.AddDays(1);
                queryable = queryable.Include(p => p.Partner).Where(s => start <= s.ScheduleDate && s.ScheduleDate <= end);
            }
            else
            {
                if(filter.PartnerName != null)
                {
                    queryable = queryable.Where(s => s.Partner.DisplayName.Contains(filter.PartnerName));
                }
                if(filter.TransactionType != null)
                {
                    TransactionType typeTrans = (TransactionType)Enum.Parse(typeof(TransactionType), filter.TransactionType);
                    queryable = queryable.Where(s => s.TransactionType.Equals(typeTrans));
                }
                if(filter.fromDate!= null)
                {
                    if (filter.fromDate.Equals(filter.toDate))
                    {
                        var dateAndTime = DateTime.Now;
                        var date = dateAndTime.Date;
                        DateTime dateFrom = DateTime.Parse(filter.fromDate).Date;
                        DateTime dateTo = dateFrom.AddDays(1).Date;
                        queryable = queryable.Where(s => dateFrom <= s.ScheduleDate && s.ScheduleDate <= dateTo);
                    }
                    else
                    {
                        if (DateTime.TryParse(filter.fromDate, out DateTime date))
                        {
                            DateTime dateFrom = DateTime.Parse(filter.fromDate).Date;
                            queryable = queryable.Where(s => s.ScheduleDate >= dateFrom);
                        }
                        if (DateTime.TryParse(filter.toDate, out DateTime date2))
                        {
                            DateTime dateTo = DateTime.Parse(filter.fromDate).Date;
                            queryable = queryable.Where(s => s.ScheduleDate >= dateTo);
                        }
                    }
                }
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

        private async Task<List<Schedule>> DoFilterHistory(String searchDate, IQueryable<Schedule> queryable, String type)
        {
            if (DateTime.TryParse(searchDate, out DateTime date))
            {
                DateTime start = DateTime.Parse(searchDate).AddDays(1);
                DateTime end = DateTime.Parse(searchDate).AddDays(2);
                queryable = queryable.Where(s => start <= s.ScheduleDate && s.ScheduleDate <= end);
            }
            if (type != null && type.Length != 0)
            {
                TransactionType typeTrans = (TransactionType)Enum.Parse(typeof(TransactionType), type);
                queryable = queryable.Where(s => s.TransactionType.Equals(typeTrans));
            }
            queryable = queryable.Where(s => !s.UpdatedBy.Contains("system"));
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
            if (filter != null)
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

        public List<ReportSchedule> GetReportSchedule(List<Schedule> listSchedule)
        {
            List<ReportSchedule> listReport = new List<ReportSchedule>();
            foreach (var item in listSchedule)
            {

            }
            return listReport;
        }

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
            Schedule currentSchedule = _dbSet.Where(s => s.ScheduleStatus == ScheduleStatus.Approved && s.TransactionType == schedule.TransactionType && s.TimeTemplateItemId == schedule.TimeTemplateItemId).SingleOrDefault();
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

        public async Task<List<Schedule>> GetBookedScheduleInDate(int partnerId)
        {
            var current = DateTime.Now.Date;
            List<Schedule> listSchedule = new List<Schedule>();
            listSchedule = await _dbSet.OrderBy(s => s.RegisteredWeight).Where(s => s.PartnerId == partnerId && s.ScheduleDate.Date == current).ToListAsync();
            return listSchedule;
        }

        public async Task<bool> UpdateActualWeight(int partnerId, float weight)
        {
            List<Schedule> listScheduled = await GetBookedScheduleInDate(partnerId);
            if (listScheduled != null && listScheduled.Count > 0)
            {
                //approximate
                if (weight < 0) weight = weight * -1;
                float deviation = (float)(weight * 0.1);
                float max = weight + deviation;
                float min = weight - deviation;

                foreach (var item in listScheduled)
                {
                    if (min < item.RegisteredWeight && item.RegisteredWeight < max)
                    {
                        item.ActualWeight = weight;
                        item.ScheduleStatus = ScheduleStatus.Success;
                        Update(item);
                        await SaveAsync();
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }
    enum SystemName
    {
        System
    }

}

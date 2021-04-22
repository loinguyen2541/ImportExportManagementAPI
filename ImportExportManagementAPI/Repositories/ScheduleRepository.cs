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


        public int CountScheduleByPartner(string dateRecord, int partnerId)
        { 
            var convert = Convert.ToDateTime(dateRecord).Date;
            return _dbSet.Where(t => t.CreatedDate >= convert && t.CreatedDate < convert.AddDays(1)
            && t.PartnerId == partnerId
            && t.ScheduleStatus == ScheduleStatus.Approved
            ).Count();
        }

        public async ValueTask<Pagination<Schedule>> GetAllAsync(PaginationParam paging, ScheduleFilterParam filter)
        {
            Pagination<Schedule> schedules = new Pagination<Schedule>();
            IQueryable<Schedule> rawData = null;
            rawData = _dbSet.Include(i => i.Partner);
            schedules = await DoFilter(paging, filter, rawData);
            return schedules;
        }

        public async ValueTask<List<Schedule>> GetHistory(string searchDate, int partnerId)
        {
            List<Schedule> schedules = new List<Schedule>();
            IQueryable<Schedule> rawData = null;
            rawData = _dbSet.Include(t => t.TimeTemplateItem);
            schedules = await DoFilterHistory(searchDate, rawData, partnerId);
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
            if (queryable != null)
            {
                switch (caseSearch)
                {
                    case 1:
                        //đặt lịch mà giao => check by realweight
                        //queryable = queryable.Where(s => s.IsCanceled == false && s.ScheduleStatus == ScheduleStatus.Success && s.ActualWeight != null);
                        break;
                    case 2:
                        //đặt lịch mà không giao => bị hủy bởi hệ thống => check by iscancel và update by hệ thống
                        //queryable = queryable.Where(s => s.IsCanceled == true && s.ScheduleStatus == ScheduleStatus.Cancel && s.UpdatedBy.Equals("system"));                        //queryable = queryable.Where(s => s.IsCanceled == true && s.ScheduleStatus == ScheduleStatus.Cancel && s.UpdatedBy.Equals("system"));
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
            DateTime start = DateTime.Now.Date;
            DateTime end = DateTime.Now.Date.AddDays(1);
            IQueryable<Schedule> rawData = _dbSet.Where(s => start <= s.ScheduleDate && s.ScheduleDate <= end && s.TransactionType.Equals(typeTrans));
            count = rawData.Count();
            return count;
        }

        public async Task<float> GetTotalSchedule(int partnerId, String searchDate)
        {
            float totalWeight = 0;
            List<Schedule> listSchedules = await GetHistory(searchDate, partnerId);
            if (listSchedules != null && listSchedules.Count != 0)
            {
                foreach (var item in listSchedules)
                {
                    totalWeight += item.RegisteredWeight;
                }
            }
            return totalWeight;
        }

        public Pagination<Schedule> DoFilterSearchPartner(ScheduleFilterParam filter, PaginationParam paging)
        {
            IQueryable<Schedule> queryable = _dbSet.Include(s => s.Partner);
            List<Schedule> schedules = new List<Schedule>();
            if (filter == null)
            {
                DateTime start = DateTime.Now;
                DateTime end = DateTime.Now.AddDays(1);
                queryable = queryable.Include(p => p.Partner).Where(s => start <= s.ScheduleDate && s.ScheduleDate <= end);
            }
            else
            {
                queryable = queryable.Where(s => !s.UpdatedBy.Contains("Update action"));
                if (filter.RegisteredWeight != 0)
                {
                    queryable = queryable.Where(s => filter.RegisteredWeight <= s.RegisteredWeight);
                }
                if (filter.PartnerName != null)
                {
                    queryable = queryable.Where(s => s.Partner.DisplayName.Contains(filter.PartnerName));
                }
                if (filter.TransactionType != null)
                {
                    TransactionType typeTrans = (TransactionType)Enum.Parse(typeof(TransactionType), filter.TransactionType);
                    queryable = queryable.Where(s => s.TransactionType.Equals(typeTrans));
                }
                if (filter.ScheduleStatus != null)
                {
                    ScheduleStatus scheduleStatus = (ScheduleStatus)Enum.Parse(typeof(ScheduleStatus), filter.ScheduleStatus);
                    queryable = queryable.Where(s => s.ScheduleStatus.Equals(scheduleStatus));
                }
                if ((filter.fromDate == DateTime.MinValue) && (filter.toDate == DateTime.MinValue))
                {
                    DateTime dateFrom = DateTime.Now;
                    DateTime dateTo = DateTime.Now.AddDays(1);
                    queryable = queryable.Where(s => dateFrom <= s.ScheduleDate && s.ScheduleDate <= dateTo);
                }
                else
                {
                    queryable = queryable.Where(s =>
                    s.ScheduleDate >= filter.fromDate
                    && s.ScheduleDate <= filter.toDate).
                    Where(s => s.ScheduleDate.Hour >= filter.fromDate.Hour
                    && s.ScheduleDate.Hour <= filter.toDate.Hour);
                    foreach (var item in queryable.ToList())
                    {
                        if (item.ScheduleDate.Hour > filter.fromDate.Hour && item.ScheduleDate.Hour < filter.toDate.Hour)
                        {
                            schedules.Add(item);
                        }   // đk nếu thỏa 2 biên giờ là  lớn hơn giờ dateFrom và nhỏ hơn giờ dateTo thì k cần so sánh phút
                        if (item.ScheduleDate.Hour == filter.fromDate.Hour)
                        {
                            if (item.ScheduleDate.Minute >= filter.fromDate.Minute)
                            {
                                schedules.Add(item);
                            }
                        }   // check biên giờ datefrom nếu bằng thì so sánh lớn hơn phút datefrom
                        if (item.ScheduleDate.Hour == filter.toDate.Hour)
                        {
                            if (item.ScheduleDate.Minute <= filter.toDate.Minute)
                            {
                                schedules.Add(item);
                            }
                        } // check biên giờ dateTo nếu bằng thì so sánh nhỏ hơn phút dateTo sau đó mới add dô
                    }   // kt giờ phút từng ngày, list này đã lọc ngày và lọc khoảng giờ 

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

            int count = schedules.Count();

            if (((paging.Page - 1) * paging.Size) > count)
            {
                paging.Page = 1;
            }

            schedules = schedules.Skip((paging.Page - 1) * paging.Size).Take(paging.Size).ToList();

            Pagination<Schedule> pagination = new Pagination<Schedule>();
            pagination.Page = paging.Page;
            pagination.Size = paging.Size;
            double totalPage = (count * 1.0) / (pagination.Size * 1.0);
            pagination.TotalPage = (int)Math.Ceiling(totalPage);
            pagination.Data = schedules.OrderByDescending(o => o.ScheduleDate).ToList();

            return pagination;
        }

        private async Task<List<Schedule>> DoFilterHistory(String searchDate, IQueryable<Schedule> queryable, int partnerId)
        {
            if (DateTime.TryParse(searchDate, out DateTime date))
            {
                var convert = Convert.ToDateTime(searchDate).Date;
                var nextDay = convert.AddDays(1);
                queryable = queryable.Where(s => convert <= s.ScheduleDate && s.ScheduleDate <= nextDay && s.ScheduleStatus == ScheduleStatus.Approved);
                int test = queryable.ToList().Count;
            }
            if (partnerId != 0)
            {
                queryable = queryable.Where(s => s.PartnerId == partnerId);
            }
            return await queryable.OrderBy(s => s.TimeTemplateItem.ScheduleTime).ToListAsync();
        }
        public async Task<List<Schedule>> GetByPartnerId(int partnerId)
        {
            List<Schedule> schedules = await _dbSet
                .Where(s => s.PartnerId == partnerId && !s.UpdatedBy.Equals("Update action")).OrderBy(s => s.ScheduleDate).ToListAsync();
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
                DateTime scheduleDate;
                if (DateTime.TryParse(filter.ScheduleDate, out scheduleDate))
                {
                    if (scheduleDate != DateTime.MinValue)
                    {
                        queryable = queryable.Where(p => p.ScheduleDate.Date == scheduleDate.Date);
                    }
                }
                if (Enum.TryParse(filter.TransactionType, out TransactionType transactionType))
                {
                    TransactionType type = (TransactionType)Enum.Parse(typeof(TransactionType), filter.TransactionType);
                    queryable = queryable.Where(p => p.TransactionType == type);
                }
            }
            queryable = queryable.Where(s => !s.UpdatedBy.Contains("Update action"));
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

        public void DisableAll()
        {
            List<Schedule> schedules = _dbSet.Where(p => p.ScheduleStatus == ScheduleStatus.Approved).ToList();
            foreach (var item in schedules)
            {
                item.ScheduleStatus = ScheduleStatus.Cancel;
                item.UpdatedBy = SystemName.System.ToString();
                _dbContext.Entry(item).State = EntityState.Modified;
            }
            Save();
        }



        public int Count()
        {
            return _dbSet.Count();
        }

        public new void Delete(Schedule schedule)
        {
            Update(schedule);
        }
        public new void Delete(object id)
        {
            Schedule schedule = _dbSet.Find(id);
            Update(schedule);
        }

        public bool Exist(int id)
        {
            return _dbSet.Any(e => e.ScheduleId == id);
        }

        public Boolean IsExist(Schedule schedule)
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

        public async Task<List<Schedule>> GetTop10Schedule(string partnerId)
        {
            IQueryable<Schedule> rawData = null;
            DateTime now = DateTime.Today;
            DateTime yesterday = now;
            DateTime tomorrow = now.AddDays(1);
            rawData = _dbSet.Include(s => s.Partner).Where(s => s.CreatedDate > yesterday && s.CreatedDate < tomorrow && s.ScheduleStatus == ScheduleStatus.Approved && !s.UpdatedBy.Equals("Update action")).OrderByDescending(o => o.ScheduleId);
            if (partnerId != null)
            {
                rawData = rawData.Where(p => p.PartnerId == int.Parse(partnerId));
            }
            return await rawData.Take(10).ToListAsync();
        }

        public async Task<List<Schedule>> GetBookedScheduleInDate(int partnerId)
        {
            var current = DateTime.Now.Date;
            List<Schedule> listSchedule = new List<Schedule>();
            listSchedule = await _dbSet.OrderBy(s => s.RegisteredWeight).
                Where(s => s.PartnerId == partnerId &&
            s.ScheduleDate.Date == current &&
            s.ScheduleStatus.Equals(ScheduleStatus.Approved)).ToListAsync();
            return listSchedule;
        }

        public async Task<bool> UpdateActualWeight(int partnerId, float weight)
        {
            if (weight < 0) weight = weight * -1;
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
        public List<ScheduleStatus> getScheduleType()
        {
            return Enum.GetValues(typeof(ScheduleStatus)).Cast<ScheduleStatus>().ToList();
        }
    }
    enum SystemName
    {
        System
    }

}

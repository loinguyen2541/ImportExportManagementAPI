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
        public async ValueTask<List<Schedule>> GetAllAsync(Paging paging, ScheduleFilter filter)
        {
            List<Schedule> schedules = new List<Schedule>();
            IQueryable<Schedule> rawData = null;
            rawData = _dbSet.Include(s => s.Partner);
            schedules = await DoFilter(filter, rawData);
            //schedules = _dbSet.ToList();
            return schedules;
        }

        private async Task<List<Schedule>> DoFilter(ScheduleFilter filter, IQueryable<Schedule> queryable)
        {
            if (filter.PartnerName != null && filter.PartnerName.Length > 0)
            {
                queryable = queryable.Where(p => p.Partner.DisplayName.Contains(filter.PartnerName));
            }
            if (DateTime.TryParse(filter.ScheduleDate, out DateTime date))
            {
                DateTime scheduleDate = DateTime.Parse(filter.ScheduleDate);
                queryable = queryable.Where(p => p.ScheduleDate == scheduleDate);
            }
            if (Enum.TryParse(filter.TransactionType, out TransactionType transactionType))
            {
                TransactionType type = (TransactionType)Enum.Parse(typeof(TransactionType), filter.TransactionType);
                queryable = queryable.Where(p => p.TransactionType == type);
            }
            return await queryable.ToListAsync();
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
    }
}
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
        public List<Schedule> GetAll(Paging paging, ScheduleFilter filter)
        {
            List<Schedule> schedules = new List<Schedule>();
            IQueryable<Schedule> rawData = null;
            schedules = DoFilter(filter, rawData);
            //schedules = _dbSet.ToList();
            return schedules;
        }

        private List<Schedule> DoFilter(ScheduleFilter filter, IQueryable<Schedule> queryable)
        {
            if (filter.PartnerName != null && filter.PartnerName.Length > 0)
            {
                queryable = _dbSet.Where(p => p.Partner.DisplayName.Contains(filter.PartnerName));
            }
            if (DateTime.TryParse(filter.ScheduleDate, out DateTime date))
            {
                DateTime scheduleDate = DateTime.Parse(filter.ScheduleDate);
                queryable = _dbSet.Where(p => p.ScheduleDate == scheduleDate);
            }
            if (Enum.TryParse(filter.TransactionType, out TransactionType transactionType))
            {
                queryable = _dbSet.Where(p => p.TransactionType.ToString() == filter.TransactionType);
            }
            return queryable.ToList();
        }
    }
}

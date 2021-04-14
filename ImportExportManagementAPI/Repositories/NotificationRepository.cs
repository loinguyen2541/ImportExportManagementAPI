using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Enums;
using ImportExportManagementAPI.Filters;
using ImportExportManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Repositories
{
    public class NotificationRepository : BaseRepository<Notification>
    {
        //get all notification
        public async ValueTask<Pagination<Notification>> GetAllNotification(PaginationParam paging)
        {
            Pagination<Notification> listNotification = new Pagination<Notification>();
            IQueryable<Notification> rawData = null;
            rawData = _dbSet.OrderBy(n => n.StatusAdmin).OrderByDescending(n => n.CreatedDate);
            listNotification = await DoPaging(paging, rawData);
            return listNotification;
        }
        //get notification of partner
        public async ValueTask<Pagination<Notification>> GetNotificationPartner(PaginationParam paging, int partnerId)
        {
            Pagination<Notification> listNotification = new Pagination<Notification>();
            IQueryable<Notification> rawData = null;
            rawData = _dbSet.OrderBy(n => n.StatusPartner).OrderByDescending(n => n.CreatedDate).Where(n => n.PartnerId == partnerId && n.StatusPartner != NotificationStatus.Unavailable);
            listNotification = await DoPaging(paging, rawData);
            return listNotification;
        }

        private async Task<Pagination<Notification>> DoPaging(PaginationParam paging, IQueryable<Notification> queryable)
        {
            if (paging.Page < 1)
            {
                paging.Page = 1;
            }

            int count = queryable.Count();
            if (paging.Size < 1 && count != 0)
            {
                paging.Size = count;
            }
            if (paging.Size < 1)
            {
                paging.Size = 1;
            }
            if (((paging.Page - 1) * paging.Size) > count)
            {
                paging.Page = 1;
            }

            queryable = queryable.Skip((paging.Page - 1) * paging.Size).Take(paging.Size);
            Pagination<Notification> pagination = new Pagination<Notification>();
            pagination.Page = paging.Page;
            pagination.Size = paging.Size;
            double totalPage = (count * 1.0) / (pagination.Size * 1.0);
            pagination.TotalPage = (int)Math.Ceiling(totalPage);
            pagination.Data = await queryable.ToListAsync();
            return pagination;
        }
    }
}

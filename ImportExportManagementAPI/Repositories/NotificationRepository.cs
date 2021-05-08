using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Enums;
using ImportExportManagementAPI.Filters;
using ImportExportManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
            rawData = _dbSet.Include(p => p.Transaction).ThenInclude(t => t.Partner).Include(i => i.Schedule).OrderBy(n => n.Status).OrderByDescending(n => n.CreatedDate);
            listNotification = await DoPaging(paging, rawData);
            return listNotification;
        }
        //get notification of partner
        public async ValueTask<Pagination<Notification>> GetNotificationPartner(PaginationParam paging, String username)
        {
            Pagination<Notification> listNotification = new Pagination<Notification>();
            IQueryable<Notification> rawData = null;
            rawData = _dbSet.Include(p => p.Transaction).ThenInclude(p => p.Partner).Include(i => i.Schedule).OrderBy(n => n.Status).OrderByDescending(n => n.CreatedDate).Where(n => n.Username == username && n.Status != NotificationStatus.Unavailable);
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

        public void PushNotification(string to, string title, string message, string urlNotificationClick)
        {
            var serverApiKey = "AAAA2xa67Uk:APA91bH6QkwHrMnYr25UQCf-Aq37uZgjVPVy2I9gVPGPmUzLUwmWRe6v3Z6KlcKXX1cy5_MPYzr6s9bsRT36D6nxNXhBxIgvR234wVVuMZXUzbUQV0398oor_CTHHNuUbLcIyryHAXHl";
            var firebaseGoogleUrl = "https://fcm.googleapis.com/fcm/send";

            var httpClient = new WebClient();
            httpClient.Headers.Add("Content-Type", "application/json");
            httpClient.Headers.Add(HttpRequestHeader.Authorization, "key=" + serverApiKey);
            var data = new
            {
                to = to,
                data = new
                {
                    notification = new
                    {
                        body = message,
                        title = title,
                        //icon = "/Content/Images/Logos/BourseVirtuelle.png",
                        url = urlNotificationClick
                    }
                },
            };

            var json = JsonConvert.SerializeObject(data);
            Byte[] byteArray = Encoding.UTF8.GetBytes(json);
            var responsebytes = httpClient.UploadData(firebaseGoogleUrl, "POST", byteArray);
            string responsebody = Encoding.UTF8.GetString(responsebytes);
            dynamic responseObject = JsonConvert.DeserializeObject(responsebody);
        }

        public async Task MakeNotificationAvailable(NotificationFilter filter)
        {
            List<Notification> listUnread = await GetListUnreadNotification(filter);
            if (listUnread != null && listUnread.Count != 0)
            {
                foreach (var item in listUnread)
                {
                    await UpdateStatusNotification(item, NotificationStatus.Available, filter);
                }
            }
        }

        public async Task<List<Notification>> GetListUnreadNotification(NotificationFilter filter)
        {
            List<Notification> notifications = new List<Notification>();
            IQueryable<Notification> queryable = _dbSet;
            if (filter != null)
            {
                //if (filter.StatusType != null)
                //    if (filter.StatusType.Equals("Admin"))
                //    {
                //        queryable = queryable.Where(n => n.Status.Equals(NotificationStatus.Unread));
                //    }
                if (filter.Username != null && filter.Username.Length > 0)
                {
                    //có truyền vào tham số partnerId
                    queryable = queryable.Where(n => n.Status.Equals(NotificationStatus.Unread) && n.Username == filter.Username);
                }
                if (filter.inputDate != DateTime.MinValue)
                {
                    //có truyền datetime
                    queryable = queryable.Where(n => DateTime.Compare(n.CreatedDate, filter.inputDate) < 0);
                }
            }
            notifications = await queryable.ToListAsync();
            return notifications;
        }

        public async Task<bool> UpdateStatusNotification(Notification noti, NotificationStatus status, NotificationFilter filter)
        {
            bool update = true;
            if (noti != null)
            {

                noti.Status = status;

                Update(noti);
                try
                {
                    await SaveAsync();
                }
                catch
                {
                    update = false;
                }
            }
            else
            {
                update = false;
            }
            return update;
        }
    }
}

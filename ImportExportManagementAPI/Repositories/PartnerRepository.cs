using ImportExportManagement_API.Models;
using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Repositories
{
    public class PartnerRepository : BaseRepository<Partner>
    {
        public async ValueTask<Pagination<Partner>> GetAllAsync(PaginationParam paging, PartnerFilter filter)
        {
            Pagination<Partner> partners = new Pagination<Partner>();
            IQueryable<Partner> rawData = null;
            rawData = _dbSet.Include(p => p.PartnerType);
            partners = await DoFilter(paging, filter, rawData);
            //schedules = _dbSet.ToList();
            return partners;
        }

        public async ValueTask<List<Partner>> GetPartnerReport(String fromDate, String toDate, int caseSearch)
        {
            List<Partner> partners = new List<Partner>();
            IQueryable<Partner> rawData = _dbSet.Include(p => p.Schedules);
            List<Partner> temp = new List<Partner>();
            temp = await rawData.ToListAsync();
            switch (caseSearch)
            {
                case 1:
                    //đặt lịch mà giao => check by realweight
                    rawData = _dbSet.Include(p => p.Schedules.Where(s => s.IsCanceled == false && s.ScheduleStatus == ScheduleStatus.Success && s.ActualWeight != null));
                    break;
                case 2:
                    //đặt lịch mà không giao => bị hủy bởi hệ thống => check by iscancel và update by hệ thống
                    rawData = _dbSet.Include(p => p.Schedules.Where(s => s.IsCanceled == true && s.ScheduleStatus == ScheduleStatus.Cancel && s.UpdatedBy.Equals("System")));
                    break;
            }
            partners = await rawData.ToListAsync();
            return partners;
        }

        private async Task<Pagination<Partner>> DoFilter(PaginationParam paging, PartnerFilter filter, IQueryable<Partner> queryable)
        {
            if (filter.Name != null && filter.Name.Length > 0)
            {
                queryable = queryable.Where(p => p.DisplayName == filter.Name);
            }
            if (filter.Email != null && filter.Email.Length > 0)
            {
                queryable = queryable.Where(p => p.Email.Contains(filter.Email));
            }
            if (filter.Phone != null && filter.Phone.Length > 0)
            {
                queryable = queryable.Where(p => p.PhoneNumber.Contains(filter.Phone));
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

            Pagination<Partner> pagination = new Pagination<Partner>();
            pagination.Page = paging.Page;
            pagination.Size = paging.Size;
            double totalPage = (count * 1.0) / (pagination.Size * 1.0);
            pagination.TotalPage = (int)Math.Ceiling(totalPage);
            pagination.Data = await queryable.ToListAsync();

            return pagination;
        }

        public List<Partner> GetPartners()
        {
            List<Partner> partners = null;
            partners = _dbSet.Include(o => o.PartnerType).Where(p => p.PartnerStatus == PartnerStatus.Active).ToList();
            return partners;
        }
        public bool Exist(int id)
        {
            return _dbSet.Any(e => e.PartnerId == id);
        }

        public void DeletePartner(Partner partner)
        {
            partner.PartnerStatus = PartnerStatus.Block;
            Update(partner);
        }
        public new void Insert(Partner partner)
        {
            Account account = new Account();

            account.Username = partner.Email;
            account.Password = "123";
            account.RoleId = 3;
            account.Status = AccountStatus.Active;
            account.Partner = partner;

            _dbContext.Account.Add(account);
            Save();
        }
        public List<PartnerStatus> GetPartnerStatus()
        {
            return Enum.GetValues(typeof(PartnerStatus)).Cast<PartnerStatus>().ToList();


        }

        public async Task<Partner> GetCards(int id)
        {
            return await _dbSet.Where(p => p.PartnerId == id).Include(p => p.IdentityCards).SingleOrDefaultAsync();
        }

        public async Task<Partner> GetPartnerByUsernameAsync(String username)
        {
            Partner partner = new Partner();
            partner = await _dbSet.Include(p => p.PartnerType).Where(p => p.Username.Equals(username)).SingleOrDefaultAsync();
            return partner;
        }
    }
}

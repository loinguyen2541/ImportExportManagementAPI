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
            rawData = _dbSet.Include(p => p.PartnerTypes);
            partners = await DoFilter(paging, filter, rawData);
            //schedules = _dbSet.ToList();
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
            partners = _dbSet.Where(p => p.PartnerStatus == PartnerStatus.Active).ToList();
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
            partner.PartnerPartnerTypes = new List<PartnerPartnerType>();
            foreach (var item in partner.PartnerTypes)
            {
                PartnerPartnerType partnerPartnerType = new PartnerPartnerType();
                partnerPartnerType.PartnerId = partner.PartnerId;
                partnerPartnerType.PartnerTypeId = item.PartnerTypeId;
                partner.PartnerPartnerTypes.Add(partnerPartnerType);
            }
            partner.PartnerTypes = null;



            _dbContext.Account.Add(account);
            Save();
        }
        public List<PartnerStatus> GetPartnerStatus()
        {
            return Enum.GetValues(typeof(PartnerStatus)).Cast<PartnerStatus>().ToList();

        }
    }
}

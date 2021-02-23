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
        public async ValueTask<List<Partner>> GetAllAsync(PartnerFilter filter)
        {
            List<Partner> partners = new List<Partner>();
            IQueryable<Partner> rawData = null;
            rawData = _dbSet.Where(p => p.PartnerStatus == PartnerStatus.Active);
            partners = await DoFilter(filter, rawData);
            //schedules = _dbSet.ToList();
            return partners;
        }

        private async Task<List<Partner>> DoFilter(PartnerFilter filter, IQueryable<Partner> queryable)
        {
            if (filter.Name != null && filter.Name.Length > 0)
            {
                queryable = queryable.Where(p => p.DisplayName.Contains(filter.Name));
            }
            if (filter.Email != null && filter.Email.Length > 0)
            {
                queryable = queryable.Where(p => p.Email.Contains(filter.Email));
            }
            if (filter.Phone != null && filter.Phone.Length > 0)
            {
                queryable = queryable.Where(p => p.PhoneNumber.Contains(filter.Phone));
            }
            return await queryable.ToListAsync();
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

    }
}

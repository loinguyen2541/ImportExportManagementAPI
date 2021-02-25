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
    public class IdentityCardRepository : BaseRepository<IdentityCard>
    {
        public async Task<Pagination<IdentityCard>> GetAllAsync(PaginationParam paging, IdentityCardFilter filter)
        {
            Pagination<IdentityCard> IdentityCards = new Pagination<IdentityCard>();
            IQueryable<IdentityCard> rawData = null;
            rawData = _dbSet.Include(p=>p.Partner);
            IdentityCards = await DoFilterAsync(paging, filter, rawData);
            //schedules = _dbSet.ToList();
            return IdentityCards;
        }

        private async Task<Pagination<IdentityCard>> DoFilterAsync(PaginationParam paging, IdentityCardFilter filter, IQueryable<IdentityCard> queryable)
        {
            if (filter.PartnerName != null && filter.PartnerName.Length > 0)
            {
                queryable = queryable.Where(p => p.Partner.DisplayName.Contains(filter.PartnerName));
            }

            if (Enum.TryParse(filter.Status, out IdentityCardStatus identityCardStatus))
            {
                queryable = queryable.Where(p => p.IdentityCardStatus == identityCardStatus);
            }
            if (filter.PartnerType != null && filter.PartnerType.Length > 0)
            {
                queryable = queryable.Where(p => p.Partner.PartnerTypes.Any(t => t.PartnerTypeName.Equals(filter.PartnerType)));
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
            Pagination<IdentityCard> pagination = new Pagination<IdentityCard>();
            pagination.Page = paging.Page;
            pagination.Size = paging.Size;
            double totalPage = (count * 1.0) / (pagination.Size * 1.0);
            pagination.TotalPage = (int)Math.Ceiling(totalPage);
            pagination.Data = await queryable.ToListAsync();
            return pagination;
        }

        public List<IdentityCard> GetIdentityCards()
        {
            List<IdentityCard> IdentityCards = null;
            IdentityCards = _dbSet.Where(p => p.IdentityCardStatus == IdentityCardStatus.Active).ToList();
            return IdentityCards;
        }
        public bool Exist(String id)
        {
            return _dbSet.Any(e => e.IdentityCardId.Equals(id));
        }

        public void DeleteIdentityCard(IdentityCard IdentityCard)
        {
            IdentityCard.IdentityCardStatus = IdentityCardStatus.Block;
            Update(IdentityCard);
        }
        public List<IdentityCardStatus> GetCardsStatus()
        {
            return Enum.GetValues(typeof(IdentityCardStatus)).Cast<IdentityCardStatus>().ToList();

        }
    }
}

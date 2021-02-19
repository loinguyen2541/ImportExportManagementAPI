using ImportExportManagement_API.Models;
using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Repositories
{
    public class IdentityCardRepository : BaseRepository<IdentityCard>
    {
        public List<IdentityCard> GetAllAsync(IdentityCardFilter filter)
        {
            List<IdentityCard> IdentityCards = new List<IdentityCard>();
            IQueryable<IdentityCard> rawData = null;
            rawData = _dbSet.Where(p => p.IdentityCardStatus == IdentityCardStatus.Active);
            IdentityCards =  DoFilter(filter, rawData);
            //schedules = _dbSet.ToList();
            return IdentityCards;
        }

        private  List<IdentityCard> DoFilter(IdentityCardFilter filter, IQueryable<IdentityCard> queryable)
        {
            if (filter.Name != null && filter.Name.Length > 0)
            {
                queryable = queryable.Where(p => p.Partner.DisplayName.Contains(filter.Name));
            }
            if (filter.Role != null && filter.Role.Length > 0)
            {
                queryable = queryable.Where(p => p.Partner.Account.Role.RoleName.Contains(filter.Role));
            }
            if (filter.Status != null && filter.Status.Length > 0)
            {
                queryable = queryable.Where(p => p.IdentityCardStatus.Equals(filter.Status));
            }
            return  queryable.ToList();
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
    }
}

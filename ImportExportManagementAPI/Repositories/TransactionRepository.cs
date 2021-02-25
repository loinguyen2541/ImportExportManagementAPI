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
    public class TransactionRepository : BaseRepository<Transaction>
    {
        public async Task<Transaction> GetByIDIncludePartnerAsync(int id)
        {
            return await _dbSet.Include(t => t.Partner).Where(t => t.TransactionId == id).FirstOrDefaultAsync();
        }
        public async ValueTask<List<Transaction>> GetAllAsync(TransactionFilter filter)
        {
            List<Transaction> listTransaction = new List<Transaction>();
            IQueryable<Transaction> rawData = null;
            rawData = _dbSet;
            listTransaction = await DoFilter(filter, rawData);
            return listTransaction;
        }

        private async Task<List<Transaction>> DoFilter(TransactionFilter filter, IQueryable<Transaction> queryable)
        {
            if (filter.PartnerName != null && filter.PartnerName.Length > 0)
            {
                queryable = queryable.Where(p => p.IdentityCard.Partner.DisplayName.Contains(filter.PartnerName));
            }
            if (DateTime.TryParse(filter.DateCreate, out DateTime date))
            {
                DateTime dateCreate = DateTime.Parse(filter.DateCreate);
                queryable = queryable.Where(p => p.TimeIn == dateCreate);
            }
            if (Enum.TryParse(filter.TransactionType, out TransactionType transactionType))
            {
                TransactionType type = (TransactionType)Enum.Parse(typeof(TransactionType), filter.TransactionType);
                queryable = queryable.Where(p => p.TransactionType == type);
            }
            return await queryable.ToListAsync();
        }
    }
}

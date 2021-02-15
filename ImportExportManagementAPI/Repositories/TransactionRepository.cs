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
        public async ValueTask<List<Transaction>> GetAllAsync(Paging paging, TransactionFilter filter)
        {
            List<Transaction> listTransaction = new List<Transaction>();
            IQueryable<Transaction> rawData = null;
            rawData = _dbSet;
            listTransaction = await DoFilter(filter, rawData);
            return listTransaction;
        }

        private async Task<List<Transaction>> DoFilter(TransactionFilter filter, IQueryable<Transaction> queryable)
        {
            if (filter.partnerName != null && filter.partnerName.Length > 0)
            {
                queryable = queryable.Where(p => p.Partner.DisplayName.Contains(filter.partnerName));
            }
            if (DateTime.TryParse(filter.dateCreate, out DateTime date))
            {
                DateTime dateCreate = DateTime.Parse(filter.dateCreate);
                queryable = queryable.Where(p => p.TimeIn == dateCreate);
            }
            return await queryable.ToListAsync();
        }
    }
}

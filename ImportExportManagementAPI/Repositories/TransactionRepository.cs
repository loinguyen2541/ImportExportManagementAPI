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
        public async ValueTask<Pagination<Transaction>> GetAllAsync(PaginationParam paging, TransactionFilter filter)
        {
            Pagination<Transaction> listTransaction = new Pagination<Transaction>();
            IQueryable<Transaction> rawData = null;
            rawData = _dbSet;
            listTransaction = await DoFilter(paging, filter, rawData);
            return listTransaction;
        }

        private async Task<Pagination<Transaction>> DoFilter(PaginationParam paging, TransactionFilter filter, IQueryable<Transaction> queryable)
        {
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


            if (filter.PartnerName != null && filter.PartnerName.Length > 0)
            {
                queryable = queryable.Where(p => p.Partner.DisplayName.Contains(filter.PartnerName));
            }
            if (DateTime.TryParse(filter.DateCreate, out DateTime date))
            {
                DateTime dateCreate = DateTime.Parse(filter.DateCreate);
                queryable = queryable.Where(p => p.CreatedDate.Date == dateCreate.Date);
            }
            if (Enum.TryParse(filter.TransactionType, out TransactionType transactionType))
            {
                TransactionType type = (TransactionType)Enum.Parse(typeof(TransactionType), filter.TransactionType);
                queryable = queryable.Where(p => p.TransactionType == type);
            }

            Pagination<Transaction> pagination = new Pagination<Transaction>();
            pagination.Page = paging.Page;
            pagination.Size = paging.Size;
            double totalPage = (count * 1.0) / (pagination.Size * 1.0);
            pagination.TotalPage = (int)Math.Ceiling(totalPage);
            pagination.Data = await queryable.ToListAsync();
            return pagination;
        }
        public async ValueTask<Pagination<Transaction>> GetTransByPartnerIdAsync(PaginationParam paging, int id)
        {
            Pagination<Transaction> listTransaction = new Pagination<Transaction>();
            IQueryable<Transaction> rawData = null;
            rawData = _dbSet.Where(t => t.PartnerId == id);
            listTransaction = await DoPaging(paging, rawData);
            return listTransaction; ;

        }
        private async Task<Pagination<Transaction>> DoPaging(PaginationParam paging, IQueryable<Transaction> queryable)
        {


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

            Pagination<Transaction> pagination = new Pagination<Transaction>();
            pagination.Page = paging.Page;
            pagination.Size = paging.Size;
            double totalPage = (count * 1.0) / (pagination.Size * 1.0);
            pagination.TotalPage = (int)Math.Ceiling(totalPage);
            pagination.Data = await queryable.ToListAsync();

            return pagination;
        }
    }
}

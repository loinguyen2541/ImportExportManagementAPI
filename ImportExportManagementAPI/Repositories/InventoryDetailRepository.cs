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
    public class InventoryDetailRepository : BaseRepository<InventoryDetail>
    {
        public async ValueTask<Pagination<InventoryDetail>> GetAllInventory(PaginationParam paging, InventoryDetailFilter filter)
        {
            Pagination<InventoryDetail> listInventoryDetails = new Pagination<InventoryDetail>();
            IQueryable<InventoryDetail> rawData = null;
            rawData = _dbSet;
            listInventoryDetails = await DoFilter(paging, filter, rawData);
            return listInventoryDetails;
        }

        private async Task<Pagination<InventoryDetail>> DoFilter(PaginationParam paging, InventoryDetailFilter filter, IQueryable<InventoryDetail> queryable)
        {
            if (filter != null)
            {
                if (filter.PartnerName != null && filter.PartnerName.Length > 0)
                {

                }
            }

            //check giá trị page client truyền
            if (paging.Page < 1)
            {
                paging.Page = 1;
            }
            if (paging.Size < 1)
            {
                paging.Size = 1;
            }

            //lấy tổng các giá trị query được
            int count = queryable.Count();

            //check giá trị page ban đầu
            if (count != 0)
            {
                if (((paging.Page - 1) * paging.Size) > count)
                {
                    paging.Page = 1;
                }
            }

            queryable = queryable.Skip((paging.Page - 1) * paging.Size).Take(paging.Size);

            Pagination<InventoryDetail> pagination = new Pagination<InventoryDetail>();
            pagination.Page = paging.Page;
            pagination.Size = paging.Size;
            double totalPage = (count * 1.0) / (pagination.Size * 1.0);
            pagination.TotalPage = (int)Math.Ceiling(totalPage);
            pagination.Data = await queryable.ToListAsync();

            return pagination;
        }

        public bool InsertInventoryDetailAsync(DateTime recordDate, Transaction trans)
        {
            InventoryRepository inventRepo = new InventoryRepository();
            var date = recordDate.Date;
            Task<Inventory> existed = inventRepo.CheckExistDateRecordAsync(date);
            if (existed.Result != null)
            {
                //nếu chưa có => tạo mới
                AddNewInventoryDetailAsync(trans, existed.Result);
                return true;
            }
            return false;
        }

        private async void AddNewInventoryDetailAsync(Transaction trans, Inventory inventory)
        {
            InventoryDetail detail = new InventoryDetail { GoodsId = 1, InventoryId = inventory.InventoryId };
            if (trans.TransactionType.Equals(TransactionType.Import))
            {
                detail.Type = InventoryDetailType.Import;
            }
            else
            {
                detail.Type = InventoryDetailType.Export;
            }

            float totalWeight = trans.WeightIn - trans.WeightOut;
            if (totalWeight < 0) totalWeight = totalWeight * -1;
            detail.Weight = totalWeight;
            Insert(detail);
            try
            {
                await SaveAsync();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}

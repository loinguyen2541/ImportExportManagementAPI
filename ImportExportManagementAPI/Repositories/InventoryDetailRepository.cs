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
        public async ValueTask<Pagination<Inventory>> GetAllInventory(PaginationParam paging, InventoryDetailFilter filter)
        {
            Pagination<Inventory> listInventory = new Pagination<Inventory>();
            IQueryable<Inventory> rawData = null;
            rawData = _dbSet;
            listInventory = await DoFilter(paging, filter, rawData);
            return listInventory;
        }

        private async Task<Pagination<Inventory>> DoFilter(PaginationParam paging, InventoryFilter filter, IQueryable<Inventory> queryable)
        {
            if (DateTime.TryParse(filter.RecordedDate, out DateTime date))
            {
                DateTime recoredDate = DateTime.Parse(filter.RecordedDate);
                queryable = queryable.Where(p => p.RecordedDate == recoredDate.Date);
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

            Pagination<Inventory> pagination = new Pagination<Inventory>();
            pagination.Page = paging.Page;
            pagination.Size = paging.Size;
            double totalPage = (count * 1.0) / (pagination.Size * 1.0);
            pagination.TotalPage = (int)Math.Ceiling(totalPage);
            pagination.Data = await queryable.ToListAsync();

            return pagination;
        }

        //public bool InsertInventory(Inventory inventory)
        //{
        //    bool existed = CheckExistDateRecord(inventory.RecordedDate);
        //    if(!existed)
        //    {
        //        //nếu chưa có => tạo mới
        //        Insert(inventory);
        //        return true;
        //    }
        //    return false;
        //}

        ////check coi này ngày đã có phiếu kiểm kho chưa
        //public bool CheckExistDateRecord(DateTime dateRecord)
        //{
        //    //true if existed
        //    return _dbSet.Any(i => i.RecordedDate.CompareTo(dateRecord) == 0);
        //}
    }
}

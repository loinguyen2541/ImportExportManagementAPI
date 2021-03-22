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
    public class InventoryRepository : BaseRepository<Inventory>
    {
        public async ValueTask<Pagination<Inventory>> GetAllInventory(PaginationParam paging, InventoryFilter filter)
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
            pagination.Data = await queryable.OrderByDescending(i => i.RecordedDate).ToListAsync();

            return pagination;
        }

        //check coi này ngày đã có phiếu kiểm kho chưa
        public async Task<Inventory> CheckExistDateRecord(DateTime dateRecord)
        {
            Task<Inventory> inventory = _dbSet.Where(i => i.RecordedDate.Equals(dateRecord)).FirstOrDefaultAsync();
            if (inventory.Result == null)
            {
                //chua co thi tao moi
                Inventory newInventory = new Inventory { RecordedDate = dateRecord };
                Insert(newInventory);
                await SaveAsync();
                return newInventory;
            }
            //co roi thi tra ve
            return inventory.Result;
        }

        public async Task<string> TotalWeightInventory(DateTime dateRecord, int type)
        {
            String total = "0.0 KG";
            //check ngày này có inventory chưa
            Inventory inventory = await CheckExistDateRecord(dateRecord);
            if (inventory != null)
            {
                //get list detail
                float weightTotal = 0;
                InventoryDetailRepository detailRepo = new InventoryDetailRepository();
                List<InventoryDetail> listDetail = await detailRepo.GetDateInventoryDetail(inventory.InventoryId, type);
                if (listDetail != null && listDetail.Count > 0)
                {
                    foreach (var item in listDetail)
                    {
                        weightTotal += item.Weight;
                    }
                }
                total = weightTotal.ToString() + " KG";
            }
            return total;
        }
        public async Task<float> TotalWeightInventoryFloat(DateTime dateRecord, int type)
        {
            float weightTotal = 0;
            //check ngày này có inventory chưa
            Inventory inventory = await CheckExistDateRecord(dateRecord);
            if (inventory != null)
            {
                //get list detail
                InventoryDetailRepository detailRepo = new InventoryDetailRepository();
                List<InventoryDetail> listDetail = await detailRepo.GetDateInventoryDetail(inventory.InventoryId, type);
                if (listDetail != null && listDetail.Count > 0)
                {
                    foreach (var item in listDetail)
                    {
                        weightTotal += item.Weight;
                    }
                }
            }
            return weightTotal;
        }
        public List<TotalInventoryDetailedByDate> TotalWeightInventoryFloatByMonth(DateTime dateFrom, DateTime dateTo)
        {
            List<TotalInventoryDetailedByDate> listDetail = new List<TotalInventoryDetailedByDate>();
            //check ngày này có inventory chưa
            if (dateFrom != null && dateTo != null)
            {
                IQueryable<Inventory> rawData = null;
                rawData = _dbSet.Where(p => p.RecordedDate >= dateFrom && p.RecordedDate <= dateTo);


                //get list detail
                List<Inventory> inventories = rawData.ToList();
                InventoryDetailRepository detailRepo = new InventoryDetailRepository();
                List<int> listInvenId = new List<int>();
                foreach (var item in inventories)
                {
                    listInvenId.Add(item.InventoryId);
                }
                if (listInvenId.Count > 0)
                {
                    listDetail = detailRepo.GetInventoryDetailDateFromDateTo(listInvenId);
                    foreach (var item in inventories)
                    {
                        foreach (var item2 in listDetail)
                        {
                            if (item.InventoryId == item2.id)
                            {
                                item2.date = item.RecordedDate;
                            }
                        }
                    }
                }

            }
            return listDetail.OrderBy(o => o.date).ToList();
        }
        public List<Inventory> ReportPartner(DateTime DateFrom, DateTime DateTo, string partnerName)
        {
            return _dbSet.Where(p => p.RecordedDate.Date >= DateFrom.Date && p.RecordedDate.Date <= DateTo.Date).Include(p => p.InventoryDetails.Where(i => i.Partner.DisplayName.Contains(partnerName))).ToList();
        }


        public Inventory ReportTransaction(DateTime currentDate, int partnerID)
        {

            return _dbSet.Where(p => p.RecordedDate == currentDate).Include(p => p.InventoryDetails.Where(i => i.Partner.PartnerId == partnerID)).ThenInclude(p => p.Goods).ThenInclude(p => p.Transactions.Where(p => p.TimeIn.Date == currentDate.Date && p.TransactionStatus == TransactionStatus.Success && p.PartnerId == partnerID)).SingleOrDefault();
        }
        public List<Inventory> ReoportInventory(DateTime DateFrom, DateTime DateTo)
        {
            return _dbSet.Where(p => p.RecordedDate.Date >= DateFrom.Date && p.RecordedDate.Date <= DateTo.Date).Include(p => p.InventoryDetails).ToList();
        }



    }
}

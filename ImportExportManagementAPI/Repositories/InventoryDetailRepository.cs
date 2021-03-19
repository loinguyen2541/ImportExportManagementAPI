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
        public async ValueTask<Pagination<InventoryDetail>> GetInventoryDetail(PaginationParam paging, InventoryDetailFilter filter)
        {
            Pagination<InventoryDetail> listInventoryDetails = new Pagination<InventoryDetail>();
            IQueryable<InventoryDetail> rawData = null;
            rawData = _dbSet.Include(i => i.Inventory).Include(i => i.Goods).Include(i => i.Partner);
            listInventoryDetails = await DoFilter(paging, filter, rawData);
            return listInventoryDetails;
        }

        private async Task<Pagination<InventoryDetail>> DoFilter(PaginationParam paging, InventoryDetailFilter filter, IQueryable<InventoryDetail> queryable)
        {
            if (filter != null)
            {
                if (filter.PartnerName != null && filter.PartnerName.Length > 0)
                {
                    queryable = queryable.Where(i => i.Partner.DisplayName.Contains(filter.PartnerName));
                }
                queryable = queryable.Where(i => i.GoodsId == 2);
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

        public async Task<bool> UpdateInventoryDetail(DateTime recordDate, Transaction trans)
        {
            InventoryRepository inventRepo = new InventoryRepository();
            var date = recordDate.Date;
            int transType = (int)trans.TransactionType;

            //check da co phieu nhao kho vao ngay chua va detail cua type nay da co chua
            Inventory checkInventoryExisted = await inventRepo.CheckExistDateRecord(date);
            Task<InventoryDetail> checkTypeExisted = CheckExistedDetailType(trans.PartnerId, transType, checkInventoryExisted.InventoryId);
            //neu inven da co && type chua co => tao moi
            if (checkTypeExisted.Result == null)
            {
                //nếu chưa có => tạo mới
                AddNewInventoryDetail(trans, checkInventoryExisted);
                return true;
            }
            else if (checkTypeExisted.Result != null)
            {
                //neu inventory da co && type da co roi => update weight
                UpdateInventoryDetailByType(trans, checkTypeExisted.Result);
                return true;
            }
            return false;
        }

        private async Task<InventoryDetail> CheckExistedDetailType(int partnerId, int type, int inventoryId)
        {
            //get list detail of partner
            List<InventoryDetail> listDetailOfPartner = await GetPartnerInventoryDetail(partnerId, inventoryId);
            if (listDetailOfPartner != null && listDetailOfPartner.Count > 0)
            {
                foreach (var item in listDetailOfPartner)
                {
                    if ((int)item.Type == type)
                    {
                        //co roi
                        return item;
                    }
                }
            }
            return null;
        }

        private async void AddNewInventoryDetail(Transaction trans, Inventory inventory)
        {
            InventoryDetail detail = new InventoryDetail { GoodsId = trans.GoodsId, InventoryId = inventory.InventoryId, PartnerId = trans.PartnerId };
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
            catch (Exception)
            {
                throw;
            }
        }

        private async void UpdateInventoryDetailByType(Transaction trans, InventoryDetail detail)
        {
            float totalWeight = trans.WeightIn - trans.WeightOut;
            if (totalWeight < 0) totalWeight = totalWeight * -1;
            detail.Weight = detail.Weight + totalWeight;
            Update(detail);
            try
            {
                await SaveAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //get list detail by partner
        private async Task<List<InventoryDetail>> GetPartnerInventoryDetail(int partnerId, int inventoryId)
        {
            List<InventoryDetail> details = new List<InventoryDetail>();
            details = await _dbSet.Where(d => d.PartnerId == partnerId && d.InventoryId == inventoryId).ToListAsync();
            return details;
        }
        //get list detail by datetime and type
        public async Task<List<InventoryDetail>> GetDateInventoryDetail(int inventoryId, int detailType)
        {
            List<InventoryDetail> details = new List<InventoryDetail>();
            details = await _dbSet.Where(d => d.InventoryId == inventoryId && (int)d.Type == detailType).ToListAsync();
            return details;
        }


        public async ValueTask<Pagination<InventoryDetail>> GetReportPartner(PaginationParam paging, InventoryFilter filter)
        {
            Pagination<InventoryDetail> listInventory = new Pagination<InventoryDetail>();
            IQueryable<InventoryDetail> rawData = null;
            rawData = _dbSet.Include(p => p.Partner);
            rawData = rawData.Include(p => p.Inventory);
            listInventory = await DoFilterReportPartner(paging, filter, rawData);
            return listInventory;
        }
        public async Task<List<InventoryDetail>> getDataReportInventoryDetail(ReportFilter filter)
        {
            List<InventoryDetail> listInventory = new List<InventoryDetail>();
            IQueryable<InventoryDetail> rawData = null;
            rawData = _dbSet.Include(p => p.Goods);
            rawData = rawData.Include(p => p.Inventory);
            listInventory = await doFilterReportInventory(filter, rawData);
            return listInventory;

        }
        private async Task<List<InventoryDetail>> doFilterReportInventory(ReportFilter filter, IQueryable<InventoryDetail> queryable)
        {
            if (DateTime.TryParse(filter.dateFrom, out DateTime FromDate) && DateTime.TryParse(filter.dateTo, out DateTime ToDate))
            {
                DateTime dateFrom = DateTime.Parse(filter.dateFrom);
                DateTime dateTo = DateTime.Parse(filter.dateTo);
                queryable = queryable.Where(p => p.Inventory.RecordedDate >= dateFrom && p.Inventory.RecordedDate <= dateTo);
            }
            return await queryable.ToListAsync();


        }

        private async Task<Pagination<InventoryDetail>> DoFilterReportPartner(PaginationParam paging, InventoryFilter filter, IQueryable<InventoryDetail> queryable)
        {

            if (filter.PartnerName != null && filter.PartnerName.Length > 0)
            {
                queryable = queryable.Where(p => p.Partner.DisplayName.Contains(filter.PartnerName));
            }

            if (DateTime.TryParse(filter.dateFrom, out DateTime FromDate) && DateTime.TryParse(filter.dateTo, out DateTime ToDate))
            {
                DateTime dateFrom = DateTime.Parse(filter.dateFrom);
                DateTime dateTo = DateTime.Parse(filter.dateTo);
                queryable = queryable.Where(p => p.Inventory.RecordedDate >= dateFrom && p.Inventory.RecordedDate <= dateTo);
            }
            if (Enum.TryParse(filter.TransactionType, out TransactionType transactionType))
            {
                InventoryDetailType type = (InventoryDetailType)Enum.Parse(typeof(InventoryDetailType), filter.TransactionType);
                queryable = queryable.Where(p => p.Type == type);
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






        //get list detail by  datefrom and dateto type 
        public  List<TotalInventoryDetailedByDate> GetInventoryDetailDateFromDateTo(List<int> inventories, int detailType)
        {
            var rawData = _dbSet.Include(p=> p.Inventory).Where(d => inventories.Contains(d.InventoryId) && (int)d.Type == detailType)
                 .GroupBy(p => p.InventoryId, (k, g) => new 
                 {
                     
                     id = k,
                     totalWeight = g.Sum(p => p.Weight)
                 }) ; ; ; ;
            List<TotalInventoryDetailedByDate> details = new List<TotalInventoryDetailedByDate>();
            foreach (var item in rawData.ToList())
            {
                TotalInventoryDetailedByDate totalInventory = new TotalInventoryDetailedByDate();
                totalInventory.id = item.id;
                totalInventory.weight = item.totalWeight;
                details.Add(totalInventory);
            }
            return details;
        }

    }
}

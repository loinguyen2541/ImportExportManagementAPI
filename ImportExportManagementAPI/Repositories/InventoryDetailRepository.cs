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

            //check da co phieu nhao kho vao ngay chua va detail cua type nay da co chua
            Inventory checkInventoryExisted = await inventRepo.CheckExistDateRecord(date);
            //check type nay partner co chua
            Task<InventoryDetail> checkTypeExisted = CheckExistedDetail(trans.PartnerId, checkInventoryExisted.InventoryId);
            //neu inven da co && type chua co => tao moi
            if (checkTypeExisted.Result == null)
            {
                //nếu chưa có => tạo mới
                return AddNewInventoryDetail(trans, checkInventoryExisted).Result;
            }
            else if (checkTypeExisted.Result != null)
            {
                //neu inventory da co && type da co roi => update weight
                return UpdateInventoryDetailByType(trans, checkTypeExisted.Result).Result;
            }
            return false;
        }

        private async Task<InventoryDetail> CheckExistedDetail(int partnerId, int inventoryId)
        {
            //get list detail of partner
            InventoryDetail detailOfPartner = await GetPartnerInventoryDetail(partnerId, inventoryId);
            if (detailOfPartner != null)
            {
                return detailOfPartner;
            }
            return null;
        }

        private async Task<Boolean> AddNewInventoryDetail(Transaction trans, Inventory inventory)
        {
            InventoryDetail detail = new InventoryDetail { GoodsId = trans.GoodsId, InventoryId = inventory.InventoryId, PartnerId = trans.PartnerId };
            float totalWeight = 0;
            if (trans.TransactionType.Equals(TransactionType.Import))
            {
                detail.Type = InventoryDetailType.Import;
                totalWeight = trans.WeightIn - trans.WeightOut;
            }
            else
            {
                detail.Type = InventoryDetailType.Export;
                totalWeight = trans.WeightOut - trans.WeightIn;
            }

            if (totalWeight < 0) totalWeight = totalWeight * -1;
            detail.Weight = totalWeight;
            Insert(detail);
            try
            {
                await SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<Boolean> UpdateInventoryDetailByType(Transaction trans, InventoryDetail detail)
        {
            float totalWeight = 0;
            if (trans.TransactionType.Equals(TransactionType.Import))
            {
                detail.Type = InventoryDetailType.Import;
                totalWeight = trans.WeightIn - trans.WeightOut;
            }
            else
            {
                detail.Type = InventoryDetailType.Export;
                totalWeight = trans.WeightOut - trans.WeightIn;
            }
            if (totalWeight < 0) totalWeight = totalWeight * -1;
            detail.Weight = detail.Weight + totalWeight;
            Update(detail);
            try
            {
                await SaveAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //get list detail by partner
        private async Task<InventoryDetail> GetPartnerInventoryDetail(int partnerId, int inventoryId)
        {
            InventoryDetail details = new InventoryDetail();
            details = await _dbSet.Where(d => d.PartnerId == partnerId && d.InventoryId == inventoryId).SingleAsync();
            return details;
        }
        //get list detail by datetime and type
        public async Task<List<InventoryDetail>> GetDateInventoryDetail(int inventoryId, InventoryDetailType detailType)
        {
            List<InventoryDetail> details = new List<InventoryDetail>();
            details = await _dbSet.Where(d => d.InventoryId == inventoryId && d.Type == detailType).ToListAsync();
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

        //public async ValueTask<List<InventoryDetail>> GetPartnerInventoryByDate(InventoryFilter filter)
        //{
        //    List<InventoryDetail> listInventory = new List<InventoryDetail>();
        //    IQueryable<InventoryDetail> rawData = null;
        //    rawData = _dbSet.Include(p => p.Partner).Include(i => i.Inventory);
        //    var date = Convert.ToDateTime(filter.SearchDate).Date;
        //    var nextDay = date.AddDays(1);

        //    rawData = _dbSet.Where(date <=  && t.CreatedDate < nextDay);
        //    return listInventory;
        //}
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

            if (DateTime.TryParse(filter.FromDate, out DateTime FromDate) && DateTime.TryParse(filter.ToDate, out DateTime ToDate))
            {
                DateTime dateFrom = DateTime.Parse(filter.FromDate);
                DateTime dateTo = DateTime.Parse(filter.ToDate);
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
        public List<TotalInventoryDetailedByDate> GetInventoryDetailDateFromDateTo(List<int> inventories)
        {
            var rawData = _dbSet.Where(d => inventories.Contains(d.InventoryId)).
               GroupBy(p => new { p.InventoryId, p.Type }, (k, g) => new TotalInventoryDetailedByDate
               {
                   id = k.InventoryId,
                   type = (InventoryDetailType)k.Type,
                   weight = g.Sum(p => p.Weight)
               }); ; ; ;

            return rawData.ToList();
        }

    }
}

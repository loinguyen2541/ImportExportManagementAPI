using ImportExportManagement_API.Models;
using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.Objects;
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
            rawData = _dbSet.Include(p => p.InventoryDetails);
            listInventory = await DoFilter(paging, filter, rawData);
            return listInventory;
        }

        private async Task<Pagination<Inventory>> DoFilter(PaginationParam paging, InventoryFilter filter, IQueryable<Inventory> queryable)
        {
            DateTime fromDate;
            DateTime toDate;
            if (DateTime.TryParse(filter.FromDate, out fromDate))
            {
                queryable = queryable.Where(p => p.RecordedDate >= fromDate.Date);
            }
            if (DateTime.TryParse(filter.ToDate, out toDate))
            {
                queryable = queryable.Where(p => p.RecordedDate <= toDate.Date);
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
            var getDate = dateRecord.Date;
            Task<Inventory> inventory = _dbSet.Where(i => i.RecordedDate.Equals(getDate)).FirstOrDefaultAsync();
            if (inventory.Result == null)
            {
                //chua co thi tao moi

                GoodsRepository goodsRepository = new GoodsRepository();
                float goodsQuantity = goodsRepository.GetGoodCapacity();
                Inventory newInventory = new Inventory { RecordedDate = dateRecord, OpeningStock = goodsQuantity };
                Insert(newInventory);
                await SaveAsync();
                return newInventory;
            }
            //co roi thi tra ve
            return inventory.Result;
        }

        //check coi partner đã có phiếu kiểm kho vào ngày này chưa
        public async Task<InventoryDetail> CheckExistDateRecordOfPartner(DateTime dateRecord, int partnerId)
        {
            var getDate = dateRecord.Date;
            Inventory inventory = await _dbSet.Where(i => i.RecordedDate.Equals(getDate)).SingleOrDefaultAsync();
            if (inventory != null)
            {
                //kiểm detail loại này của partner trong này đã có chưa
                InventoryDetail detailByDate = await _dbContext.InventoryDetail.Where(d => d.InventoryId == inventory.InventoryId && d.PartnerId == partnerId).SingleOrDefaultAsync();
                return detailByDate;
            }
            //chua co tra ve null
            return null;
        }

        public async Task<string> TotalWeightInventory(DateTime dateRecord, InventoryDetailType type)
        {
            String total = "0.0 kg";
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
                total = weightTotal.ToString() + " kg";
            }
            return total;
        }
        public async Task<ObjectTotalImportExportToday> TotalWeightInventoryFloat(DateTime dateRecord)
        {
            //check ngày này có inventory chưa
            Inventory inventory = await CheckExistDateRecord(dateRecord);
            ObjectTotalImportExportToday objectTotal = new ObjectTotalImportExportToday();
            if (inventory != null)
            {
                float weightTotal = 0;
                //get list detail
                InventoryDetailRepository detailRepo = new InventoryDetailRepository();
                List<InventoryDetail> listImport = await detailRepo.GetDateInventoryDetail(inventory.InventoryId, InventoryDetailType.Import);
                List<InventoryDetail> listExport = await detailRepo.GetDateInventoryDetail(inventory.InventoryId, InventoryDetailType.Export);
                if (listImport != null && listImport.Count > 0)
                {
                    foreach (var item in listImport)
                    {
                        weightTotal += item.Weight;
                    }
                    objectTotal.Import = weightTotal;
                }
                weightTotal = 0;
                if (listExport != null && listExport.Count > 0)
                {
                    foreach (var item in listExport)
                    {
                        weightTotal += item.Weight;
                    }
                    objectTotal.Export = weightTotal;
                }
                objectTotal.OpeningStock = GetOpeningStockByDate(dateRecord, inventory.InventoryId);
                objectTotal.Iventory = (objectTotal.OpeningStock + objectTotal.Import) - objectTotal.Export;
            }
            return objectTotal;
        }
        public float GetOpeningStockByDate(DateTime date, int id)
        {
            float OpeningStock = _dbSet.Where(o => o.RecordedDate == date && o.InventoryId == id).Select(o => o.OpeningStock).FirstOrDefault();
            return OpeningStock;
        }
        public List<TotalInventoryDetailedByDate> TotalWeightInventoryFloatByMonth(DateTime dateFrom, DateTime dateTo)
        {
            List<TotalInventoryDetailedByDate> listDetail = new List<TotalInventoryDetailedByDate>();
            //check ngày này có inventory chưa
            if (dateFrom != DateTime.MinValue && dateTo != DateTime.MinValue)
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
            return _dbSet.Where
                (p => p.RecordedDate >= DateFrom && p.RecordedDate <= DateTo).
                Include(p => p.InventoryDetails.Where(p=>p.Partner.DisplayName == partnerName))
                .ThenInclude(p=> p.Partner).ThenInclude(p => p.Transactions).ToList();
        }


        public List<Inventory> ReoportInventory(DateTime DateFrom, DateTime DateTo)
        {
            return _dbSet.Where(p => p.RecordedDate.Date >= DateFrom.Date && p.RecordedDate.Date <= DateTo.Date).Include(p => p.InventoryDetails).ToList();
        }

        //lấy tổng khối lượng nhập/xuất theo ngày của partner
        public async Task<float> TotalWeightInventoryOfPartnerByDate(DateTime dateRecord, int partnerId)
        {
            float total = 0;
            InventoryDetail detail = await CheckExistDateRecordOfPartner(dateRecord, partnerId);
            if (detail != null)
            {
                total = detail.Weight;
            }
            return total;
        }

        //lấy tổng khối lượng nhập/xuất theo khoảng thời gian của partner
        public async Task<float> TotalWeightInventoryOfPartnerByTime(DateTime fromDate, DateTime toDate, int partnerId)
        {
            float total = 0;

            //lấy list inventory trong khoảng thgian này
            List<Inventory> listInventory = new List<Inventory>();
            listInventory = await _dbSet.Where(i => fromDate <= i.RecordedDate && i.RecordedDate <= toDate).ToListAsync();
            if (listInventory.Count != 0)
            {
                List<InventoryDetail> listDetail = new List<InventoryDetail>();
                List<InventoryDetail> temp = null;
                foreach (var item in listInventory)
                {
                    temp = await _dbContext.InventoryDetail.Where(d => d.InventoryId == item.InventoryId && d.PartnerId == partnerId).ToListAsync();
                    if (temp != null)
                    {
                        foreach (var itemDetail in temp)
                        {
                            listDetail.Add(itemDetail);
                        }
                    }
                    temp = null;
                }

                if (listDetail.Count != 0)
                {
                    foreach (var item in listDetail)
                    {
                        total += item.Weight;
                    }
                }
            }
            return total;
        }

        //lấy list import/export trong khoảng thời gian
        public async Task<List<InventoryDetail>> GetImportExportByTime(DateTime fromDate, DateTime toDate, int partnerId)
        {
            //lấy list inventory trong khoảng thgian này
            List<Inventory> listInventory = new List<Inventory>();
            List<InventoryDetail> listDetail = new List<InventoryDetail>();
            fromDate = fromDate.Date;
            toDate = toDate.Date;
            listInventory = await _dbSet.Where(i => fromDate <= i.RecordedDate && i.RecordedDate <= toDate).ToListAsync();
            if (listInventory.Count != 0)
            {
                List<InventoryDetail> temp = null;
                foreach (var item in listInventory)
                {
                   if(partnerId != 0) temp = await _dbContext.InventoryDetail.Where(d => d.InventoryId == item.InventoryId && d.PartnerId == partnerId).ToListAsync();
                    else temp = await _dbContext.InventoryDetail.Where(d => d.InventoryId == item.InventoryId).ToListAsync();
                    if (temp.Count != 0)
                    {
                        foreach (var itemDetail in temp)
                        {
                            listDetail.Add(itemDetail);
                        }
                    }
                    temp = null;
                }
            }
            return listDetail;
        }

    }
}

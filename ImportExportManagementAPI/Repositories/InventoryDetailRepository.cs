﻿using ImportExportManagement_API.Models;
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
                if (filter.InventoryId != 0)
                {
                    queryable = queryable.Where(i => i.InventoryId == filter.InventoryId);
                }
                if (filter.PartnerName != null && filter.PartnerName.Length > 0)
                {
                    queryable = queryable.Where(i => i.Partner.DisplayName.Contains(filter.PartnerName));
                }
                queryable = queryable.Where(i => i.GoodsId == 1);
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
            Task<Inventory> checkInventoryExisted = inventRepo.CheckExistDateRecord(date);
            Task<InventoryDetail> checkTypeExisted = CheckExistedDetailType(trans.PartnerId, transType);
            await Task.WhenAll(checkInventoryExisted, checkTypeExisted);
            //neu inven da co && type chua co => tao moi
            if ((checkInventoryExisted.Result != null) && (checkTypeExisted.Result == null))
            {
                //nếu chưa có => tạo mới
                AddNewInventoryDetail(trans, checkInventoryExisted.Result);
                return true;
            }
            else if ((checkInventoryExisted.Result != null) && (checkTypeExisted.Result != null))
            {
                //neu inventory da co && type da co roi => update weight
                UpdateInventoryDetailByType(trans, checkTypeExisted.Result);
                return true;
            }
            return false;
        }

        private async Task<InventoryDetail> CheckExistedDetailType(int partnerId, int type)
        {
            //get list detail of partner
            List<InventoryDetail> listDetailOfPartner = await GetPartnerInventoryDetail(partnerId);
            if(listDetailOfPartner!= null && listDetailOfPartner.Count > 0)
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
            InventoryDetail detail = new InventoryDetail { GoodsId = 2, InventoryId = inventory.InventoryId, PartnerId = trans.PartnerId };
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
            catch (Exception e)
            {
                throw;
            }
        }

        //get list detail by partner
        private async Task<List<InventoryDetail>> GetPartnerInventoryDetail(int partnerId)
        {
            List<InventoryDetail> details = new List<InventoryDetail>();
            details = await _dbSet.Where(d => d.PartnerId == partnerId).ToListAsync();
            return details;
        }
        //get list detail by datetime and type
        public async Task<List<InventoryDetail>> GetDateInventoryDetail(int inventoryId, int detailType)
        {
            List<InventoryDetail> details = new List<InventoryDetail>();
            details = await _dbSet.Where(d => d.InventoryId == inventoryId && (int)d.Type == detailType).ToListAsync();
            return details;
        }
    }
}

using ImportExportManagement_API.Models;
using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
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

        //kiểm tra xem các trans đang process có thẻ này không
        public async Task<bool> CheckProcessingCard(String cardId, String method)
        {
            bool check = false;
            List<Transaction> listProcessingTrans = new List<Transaction>();
            listProcessingTrans = _dbSet.Where(t => t.TransactionStatus.Equals(TransactionStatus.Progessing)).ToList();
            if(listProcessingTrans!= null && listProcessingTrans.Count != 0)
            {
                if (method.Equals("Insert"))
                {
                    //insert
                    check = await DisableProcessingTransInInsert(listProcessingTrans, cardId);
                }
                else if (method.Equals("UpdateArduino"))
                {
                    //update by arduino
                    check = await DisableProcessingInUpdateByArduino(listProcessingTrans, cardId);
                }
            }
            return check;
        }
        public async Task<bool> UpdateTransScandCardAsync(String cardId, float weightOut, DateTime timeOut)
        {
            bool check = false;
            //disable những transaction của thẻ này trước đó đang ở trạng thái processing => trừ cái mới nhất để update
            bool checkProcessingCard = await CheckProcessingCard(cardId, "Update");
            if (!checkProcessingCard)
            {
                //tìm transaction gần nhất của thẻ ở trạng thái processing
                var trans = FindTransToWeightOut(cardId);
                if (trans != null)
                {
                    //tìm thấy trans nhưng weightIn = 0 => transaction ko hợp lệ
                    if (trans.WeightIn == 0)
                    {
                        check = false;
                    }
                    else
                    {
                        if (weightOut != 0)
                        {
                            //set type
                            SetTransactionType(trans, weightOut);
                            //set time out
                            trans.TimeOut = timeOut;
                            //set weight out
                            trans.WeightOut = weightOut;
                            //change status
                            trans.TransactionStatus = TransactionStatus.Success;
                            //update transaction
                            try
                            {
                                Update(trans);
                                Task saveDB = SaveAsync();
                                //tạo inventory detail
                                Task updateDetail = UpdateInventoryDetail(trans);
                                check = true;
                            }
                            catch
                            {
                                check = false;
                            }
                        }
                    }
                }
            }
            return check;
        }

        //insert method => disable all transation processing in 
        public async Task<bool> DisableProcessingTransInInsert(List<Transaction> listDisable, String cardId)
        {
            bool check = false;
            for (int i = 0; i < listDisable.Count; i++)
            {
                if (listDisable[i].IdentityCardId != null)
                {
                    //insert => disable all processing transaction
                    if (listDisable[i].IdentityCardId.Equals(cardId))
                    {
                        check = await UpdateStatusProcessingTransactionAsync(listDisable[i]);
                        if (check)
                        {
                            check = false;
                        }
                    }
                }
            }
            return check;
        }
        //update method => disable transations processing in expect last
        public async Task<bool> DisableProcessingTransInUpdateManual(List<Transaction> listDisable, String cardId, int transId)
        {
            bool check = false;
            if (listDisable != null && listDisable.Count != 0)
            {
                for (int i = 0; i < listDisable.Count; i++)
                {
                    if (listDisable[i].IdentityCardId != null)
                    {
                        //update => disable all processing transaction except it
                        if (listDisable[i].IdentityCardId.Equals(cardId) && (listDisable[i].TransactionId != transId))
                        {
                            check = await UpdateStatusProcessingTransactionAsync(listDisable[i]);
                            if (check)
                            {
                                check = false;
                            }
                        }
                    }
                }
            }
            return check;
        }
        //update method => disable transations processing in expect last
        public async Task<bool> DisableProcessingInUpdateByArduino(List<Transaction> listDisable, String cardId)
        {
            bool check = false;
            if (listDisable != null && listDisable.Count != 0)
            {
                for (int i = 0; i < listDisable.Count; i++)
                {
                    if (listDisable[i].IdentityCardId != null)
                    {
                        //update => disable all processing transaction except it
                        if (listDisable[i].IdentityCardId.Equals(cardId) && (i != (listDisable.Count - 1)))
                        {
                            check = await UpdateStatusProcessingTransactionAsync(listDisable[i]);
                            if (check)
                            {
                                check = false;
                            }
                        }
                    }
                }
            }
            return check;
        }
        //disable status processing transaction
        private async Task<bool> UpdateStatusProcessingTransactionAsync(Transaction trans)
        {
            bool update = true;
            if (trans != null)
            {
                trans.TransactionStatus = TransactionStatus.Disable;
                Update(trans);
                try
                {
                    await SaveAsync();
                }
                catch
                {
                    update = false;
                }
            }
            else
            {
                update = false;
            }
            return update;
        }

        public async Task<List<Transaction>> GetTransactionByInventoryDetail(int detailId)
        {
            List<Transaction> listTransaction = new List<Transaction>();
            IQueryable<Transaction> rawData = null;
            InventoryDetail detail = (InventoryDetail)_dbContext.InventoryDetail.Find(detailId);
            Inventory inventory = (Inventory)_dbContext.Inventory.Find(detail.InventoryId);
            rawData = _dbSet.Include(t => t.Partner);
            rawData = _dbSet.Where(t => t.CreatedDate.CompareTo(inventory.RecordedDate) == 0 && t.PartnerId == detail.PartnerId && t.TransactionType.Equals(detail.Type));
            listTransaction = await rawData.ToListAsync();
            return listTransaction;
        }
        public async ValueTask<Pagination<Transaction>> GetAllAsync(PaginationParam paging, TransactionFilter filter)
        {
            Pagination<Transaction> listTransaction = new Pagination<Transaction>();
            IQueryable<Transaction> rawData = null;
            rawData = _dbSet.Include(t => t.Partner);
            listTransaction = await DoFilter(paging, filter, rawData);
            return listTransaction;
        }

        public async ValueTask<Pagination<Transaction>> GetLastIndex(PaginationParam paging)
        {
            Pagination<Transaction> listTransaction = new Pagination<Transaction>();
            IQueryable<Transaction> rawData = null;
            rawData = _dbSet.OrderByDescending(t => t.TransactionId).Where(p => p.TransactionStatus.Equals(TransactionStatus.Progessing));
            listTransaction = await DoFilter(paging, null, rawData);
            return listTransaction;
        }

        private async Task<Pagination<Transaction>> DoFilter(PaginationParam paging, TransactionFilter filter, IQueryable<Transaction> queryable)
        {

            if (filter != null)
            {
                
                if ((DateTime.TryParse(filter.DateFrom, out DateTime dateFrom) && (DateTime.TryParse(filter.DateTo, out DateTime dateTo) )))
                {
                    DateTime fromDate = DateTime.Parse(filter.DateFrom);
                    DateTime toDate = DateTime.Parse(filter.DateTo);
                    queryable = queryable.Where(p => p.CreatedDate.Date > fromDate && p.CreatedDate.Date < toDate);
                }
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
            }
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

        //tìm transaction mới nhất của thẻ ở trạng thái processing
        public Transaction FindTransToWeightOut(String cardId)
        {
            Transaction existed = _dbSet.OrderBy(t => t.TransactionId).Where(t => t.IdentityCardId.Equals(cardId) && t.TransactionStatus.Equals(TransactionStatus.Progessing)).LastOrDefault();
            return existed;
        }

        /*
         * update transaction when scand card secondth => truyền vào transaction cần update
         * check coi
         * check weight để insert datetype
         * update date weight in weight out
         */


        //identity transaction type
        public void SetTransactionType(Transaction trans, float weightOut)
        {
            float totalWeight = trans.WeightIn - weightOut;
            if (totalWeight > 0)
            {
                //nhập kho
                trans.TransactionType = TransactionType.Import;
            }
            else
            {
                //xuất kho
                trans.TransactionType = TransactionType.Export;
            }
        }

        //public async Task<bool> CreateTransactionAsync(Transaction trans, String method)
        //{
        //    bool checkCreate = false;
        //    DateTime dateTimeNow = DateTime.Now;
        //    trans.CreatedDate = dateTimeNow;
        //    //tạo bằng arduino => mã thẻ quẹt
        //    //tạo bằng tay => thẻ của bv
        //    if (trans.IdentityCardId != null)
        //    {
        //        //disable hết các transaction của thẻ này mà đang status processing
        //        bool checkProcessingCard = await CheckProcessingCard(trans.IdentityCardId, "Insert");
        //        if (!checkProcessingCard)
        //        {
        //            if (trans.WeightIn > 0)
        //            {
        //                if (method.Equals("manual"))
        //                {
        //                    //tạo bằng tay => yêu cầu nhập cả time in, time out và status phải là success
        //                    if (trans.WeightOut != 0 && trans.TimeOut != null && trans.TransactionStatus.Equals(TransactionStatus.Success))
        //                    {
        //                        //check hợp lệ => tạo transaction
        //                        SetTransactionType(trans, trans.WeightOut);
        //                        Insert(trans);
        //                        checkCreate = true;
        //                        //tạo transaction thành công => tạo inventory detail
        //                        await UpdateInventoryDetail(trans);
        //                    }
        //                    else
        //                    {
        //                        checkCreate = false;
        //                    }
        //                }
        //                else
        //                {
        //                    //tạo bằng arduino => xác phải có vật trên cân
        //                    Insert(trans);
        //                    checkCreate = true;
        //                }

        //            }
        //        }
        //    }
        //    return checkCreate;
        //}

        //update transaction
        public async Task<bool> UpdateTransactionByManual(Transaction trans, int id)
        {
            bool checkUpdate = true;
            if(id!= trans.TransactionId)
            {
                return checkUpdate = false;
            }
            Update(trans);
            try
            {
                await SaveAsync();
            }
            catch(Exception )
            {
                checkUpdate = false;
            }
            return checkUpdate;
        }
        public async Task<bool> UpdateTransactionArduino(String cardId, float weightOut, String method)
        {
            bool checkUpdate = true;
            Partner partner;
            if (cardId!= null && cardId.Length > 0) //insert by arduino
            {
                //find provider and check card status
                IdentityCardRepository cardRepo = new IdentityCardRepository();
                Task<IdentityCard> checkCard = cardRepo.checkCard(cardId);
                if (checkCard.Result == null)
                {
                    //card not available
                    return checkUpdate = false;
                }
                partner = cardRepo.GetPartnerCard(checkCard.Result.PartnerId).Result;
            }
            else
            {
                return checkUpdate = false;
            }
            //get partner failed
            if (partner == null)
            {
                return checkUpdate = false;
            }


            bool checkProcessingCard = await CheckProcessingCard(cardId, method); ;

            if (checkProcessingCard)
            {
                return checkUpdate = false;
            }
            else
            {
                if (weightOut <= 0) return checkUpdate = false;
                var trans = FindTransToWeightOut(cardId);
                if (trans.WeightIn <= 0) return checkUpdate = false;

                //set type
                SetTransactionType(trans, weightOut);
                //set time out
                trans.TimeOut = DateTime.Now;
                //set weight out
                trans.WeightOut = weightOut;
                //change status
                trans.TransactionStatus = TransactionStatus.Success;
                //update transaction
                Update(trans);
                try
                {
                    await SaveAsync();
                    //update transaction thành công => tạo inventory detail
                    await UpdateInventoryDetail(trans);
                }
                catch (Exception )
                {
                    if (GetByID(trans.TransactionId) == null)
                    {
                        return checkUpdate = false;
                    }
                    else
                    {
                        return checkUpdate = false;
                    }
                }
            }

            return checkUpdate;
        }

        //tao inventory detail
        private async Task UpdateInventoryDetail(Transaction trans)
        {
            InventoryDetailRepository detailRepo = new InventoryDetailRepository();
            await detailRepo.UpdateInventoryDetail(trans.CreatedDate, trans);
        }

        //tạo transaction
        public async Task<Transaction> CreateTransaction(Transaction trans, String method)
        {
            //check validate weight in weight out
            if (trans.WeightIn <= 0)
            {
                return null;
            }
            if (method.Equals("manual"))
            {
                if (trans.WeightOut <= 0)
                {
                    return null;
                }
            }

            //check card || provider
            Partner partner;
            if (trans.IdentityCardId != null) //insert by arduino
            {
                //find provider and check card status
                IdentityCardRepository cardRepo = new IdentityCardRepository();
                Task<IdentityCard> checkCard = cardRepo.checkCard(trans.IdentityCardId);
                if (checkCard.Result == null)
                {
                    //card not available
                    return null;
                }
                partner = cardRepo.GetPartnerCard(checkCard.Result.PartnerId).Result;
            }
            else
            {
                partner = (Partner)_dbContext.Partner.Where(p => p.PartnerId == trans.PartnerId && p.PartnerStatus.Equals(PartnerStatus.Active));
            }
            //get partner failed
            if (partner == null)
            {
                return null;
            }

            bool checkProceesingCard = await CheckProcessingCard(trans.IdentityCardId, "Insert");
            if (checkProceesingCard) return null;

            //check hợp lệ => tạo transaction
            trans.PartnerId = partner.PartnerId;
            trans.GoodsId = _dbContext.Goods.First().GoodsId;
            SetTransactionType(trans, trans.WeightOut);
            Insert(trans);
            if (trans.TransactionStatus.Equals(TransactionStatus.Success))
            {
                //tạo transaction thành công => tạo inventory detail
                await UpdateInventoryDetail(trans);
            }
            return trans;
        }
    }
}

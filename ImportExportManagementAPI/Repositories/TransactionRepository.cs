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
        public async Task<bool> CheckProcessingCard(String cardId, String method, int transId)
        {
            bool check = false;
            IQueryable<Transaction> rawData = null;
            List<Transaction> listProcessingTrans = new List<Transaction>();
            rawData = _dbSet;
            rawData = rawData.Where(t => t.TransactionStatus.Equals(TransactionStatus.Progessing));
            listProcessingTrans = await rawData.ToListAsync();
            if (transId == 0)
            {
                if (method.Equals("Insert") || method.Equals("CheckCard"))
                {
                    //insert
                    check = await DisableProcessingTransInInsert(listProcessingTrans, cardId);
                }
                else if (method.Equals("Update"))
                {
                    //update by arduino
                    check = await DisableProcessingTransInUpdate(listProcessingTrans, cardId, transId);
                }
            }
            else
            {
                check = await DisableProcessingTransInUpdate(listProcessingTrans, cardId, transId);
            }
            return check;
        }
        //insert method => disable all transation processing in 
        public async Task<bool> DisableProcessingTransInInsert(List<Transaction> listDisable, String cardId)
        {
            bool check = false;
            if (listDisable != null && listDisable.Count != 0)
            {
                for (int i = 0; i < listDisable.Count; i++)
                {
                    if (listDisable[i].IdentityCardId != null)
                    {
                        //insert => disable all processing transaction
                        if (listDisable[i].IdentityCardId.Equals(cardId))
                        {
                            check = true;
                            if (check)
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
            }
            return check;
        }
        //update method => disable transations processing in expect last
        public async Task<bool> DisableProcessingTransInUpdate(List<Transaction> listDisable, String cardId, int transId)
        {
            bool check = false;
            if (listDisable != null && listDisable.Count != 0)
            {
                for (int i = 0; i < listDisable.Count; i++)
                {
                    if (listDisable[i].IdentityCardId != null)
                    {
                        //insert => disable all processing transaction
                        if (listDisable[i].IdentityCardId.Equals(cardId) && (listDisable[i].TransactionId != transId) && (i!=(listDisable.Count - 1)))
                        {
                            check = true;
                            if (check)
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

        public async Task<bool> UpdateTransScandCardAsync(String cardId, float weightOut, DateTime timeOut)
        {
            bool check = false;
            bool checkProcessingCard = await CheckProcessingCard(cardId, "Update", 0);
            if (!checkProcessingCard)
            {
                var trans = FindTransToWeightOut(cardId);
                if (trans != null)
                {
                    if (trans.TimeIn == null || trans.WeightIn == null || trans.WeightIn == 0)
                    {
                        check = false;
                    }
                    else
                    {
                        if (weightOut != 0 && timeOut != null)
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
                                await SaveAsync();
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

        public async Task<bool> CreateTransactionAsync(Transaction trans, String method)
        {
            DateTime dateTimeNow = DateTime.Now;
            trans.CreatedDate = dateTimeNow;
            if (trans.IdentityCardId != null)
            {
                bool checkProcessingCard = await CheckProcessingCard(trans.IdentityCardId, "Insert", 0);
                if (!checkProcessingCard)
                {
                    if (trans.WeightIn > 0 && trans.TimeIn != null)
                    {
                        if (method.Equals("manual"))
                        {
                            if (trans.WeightOut != 0 && trans.TimeOut != null && trans.TransactionStatus.Equals(TransactionStatus.Success))
                            {
                                SetTransactionType(trans, trans.WeightOut);
                                Insert(trans);
                                return true;
                            }
                            return false;
                        }
                        else
                        {
                            Insert(trans);
                            return true;
                        }

                    }
                }
            }
            return false;
        }

        //update transaction
        public async Task<String> UpdateTransaction(Transaction trans, int id)
        {
            String checkUpdate = "";
            if (id != trans.TransactionId)
            {
                checkUpdate = "Invalid input";
            }

            bool checkProcessingCard = await CheckProcessingCard(trans.IdentityCardId,"Update", trans.TransactionId);

            if (checkProcessingCard)
            {
                checkUpdate = "Update processing transactions failed";
            }
            else
            {
                Update(trans);
                try
                {
                    await SaveAsync();
                }
                catch (Exception e)
                {
                    if (GetByID(trans.TransactionId) == null)
                    {
                        checkUpdate = "Transaction is not exist";
                    }
                    else
                    {
                        checkUpdate = e.Message;
                    }
                }
            }

            return checkUpdate;
        }
    }
}

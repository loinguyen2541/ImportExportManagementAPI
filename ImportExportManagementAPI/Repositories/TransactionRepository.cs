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
        public async ValueTask<List<Transaction>> GetAllAsync(TransactionFilter filter)
        {
            List<Transaction> listTransaction = new List<Transaction>();
            IQueryable<Transaction> rawData = null;
            rawData = _dbSet;
            listTransaction = await DoFilter(filter, rawData);
            return listTransaction;
        }

        private async Task<List<Transaction>> DoFilter(TransactionFilter filter, IQueryable<Transaction> queryable)
        {
            if (filter.partnerName != null && filter.partnerName.Length > 0)
            {
                queryable = queryable.Where(p => p.IdentityCard.Partner.DisplayName.Contains(filter.partnerName));
            }
            if (DateTime.TryParse(filter.dateCreate, out DateTime date))
            {
                DateTime dateCreate = DateTime.Parse(filter.dateCreate);
                queryable = queryable.Where(p => p.TimeIn == dateCreate);
            }
            return await queryable.ToListAsync();
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

        public bool UpdateTransScandCard(String cardId, float weightOut, DateTime timeOut)
        {
            bool check = false;
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
                        //check type
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
                            check = true;
                        }
                        catch
                        {
                            check = false;
                        }
                    }
                }
            }
            return check;
        }

        public bool CreateTransaction(Transaction trans, String method)
        {
            DateTime dateTimeNow = DateTime.Now;
            trans.CreatedDate = dateTimeNow;
            if (trans.WeightIn != 0 && trans.TimeIn != null)
            {
                if (method.Equals("manual"))
                {
                    if(trans.WeightOut != 0 && trans.TimeOut != null && trans.TransactionStatus.Equals(TransactionStatus.Success))
                    {
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
            return false;
        }
    }
}

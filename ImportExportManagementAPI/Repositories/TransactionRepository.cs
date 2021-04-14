using ClosedXML.Excel;
using ImportExportManagement_API.Models;
using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Helper;
using ImportExportManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;
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
            if (listProcessingTrans != null && listProcessingTrans.Count != 0)
            {
                if (method.Equals("Insert"))
                {
                    //insert
                    check = await DisableProcessingTransInInsert(listProcessingTrans, cardId);
                }
                else if (method.Equals("Update"))
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
                if (listDisable[i].IdentificationCode != null)
                {
                    //insert => disable all processing transaction
                    if (listDisable[i].IdentificationCode.Equals(cardId))
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
                    if (listDisable[i].IdentificationCode != null)
                    {
                        //update => disable all processing transaction except it
                        if (listDisable[i].IdentificationCode.Equals(cardId) && (listDisable[i].TransactionId != transId))
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
                    if (listDisable[i].IdentificationCode != null)
                    {
                        //update => disable all processing transaction except it
                        if (listDisable[i].IdentificationCode.Equals(cardId) && (i != (listDisable.Count - 1)))
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
                trans.TimeOut = DateTime.Now;
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
            rawData = _dbSet.Include(t => t.Partner).OrderByDescending(t => t.CreatedDate);
            listTransaction = await DoFilter(paging, filter, rawData);
            return listTransaction;
        }
        /*  public Pagination<TopPartner> GetTopPartner(PaginationParam paging, TransactionFilter filter)
          {
              Pagination<TopPartner> listTopPartner = new Pagination<TopPartner>();
              IQueryable<Transaction> rawData = null;
              rawData = _dbSet.Include(t => t.Partner).Where(p => p.TransactionStatus == TransactionStatus.Success);
              listTopPartner = DoFilterTop10(paging, filter, rawData);
              return listTopPartner;
          }*/
        public async ValueTask<Pagination<Transaction>> GetLastIndex(PaginationParam paging)
        {
            Pagination<Transaction> listTransaction = new Pagination<Transaction>();
            IQueryable<Transaction> rawData = null;
            rawData = _dbSet.OrderByDescending(t => t.CreatedDate).Where(p => p.TransactionStatus.Equals(TransactionStatus.Progessing));
            listTransaction = await DoFilter(paging, null, rawData);
            return listTransaction;
        }

        public async ValueTask<Pagination<Transaction>> GetLastesOfPartner(PaginationParam paging, int partnerId)
        {
            Pagination<Transaction> listTransaction = new Pagination<Transaction>();
            IQueryable<Transaction> rawData = null;
            rawData = _dbSet.OrderByDescending(t => t.CreatedDate).Where(p => p.PartnerId == partnerId);
            listTransaction = await DoFilter(paging, null, rawData);
            return listTransaction;
        }

        private async Task<Pagination<Transaction>> DoFilter(PaginationParam paging, TransactionFilter filter, IQueryable<Transaction> queryable)
        {

            if (filter != null)
            {
                if (filter.TransactionStatus != null)
                {
                    TransactionStatus trans;
                    if (Enum.TryParse(filter.TransactionStatus, out trans))
                        queryable = queryable.Where(t => t.TransactionStatus == trans);
                }
                if ((DateTime.TryParse(filter.DateFrom, out DateTime dateFrom) && (DateTime.TryParse(filter.DateTo, out DateTime dateTo))))
                {
                    if (filter.DateFrom.Equals(filter.DateTo))
                    {
                        var convert = Convert.ToDateTime(filter.DateFrom).Date;
                        var nextDay = convert.AddDays(1);
                        queryable = queryable.Where(t => convert <= t.CreatedDate && t.CreatedDate < nextDay);
                    }
                    else
                    {
                        DateTime fromDate = DateTime.Parse(filter.DateFrom);
                        DateTime toDate = DateTime.Parse(filter.DateTo);
                        queryable = queryable.Where(t => fromDate <= t.CreatedDate.Date && t.CreatedDate.Date <= toDate);

                    }
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
                if (filter.PartnerId != 0)
                {
                    queryable = queryable.Where(p => p.PartnerId == filter.PartnerId);
                    Console.WriteLine(queryable.ToList().Count);
                }
            }
            if (paging.Page < 1)
            {
                paging.Page = 1;
            }

            int count = queryable.Count();
            if (paging.Size < 1 && count != 0)
            {
                paging.Size = count;
            }
            if (paging.Size < 1)
            {
                paging.Size = 1;
            }
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

        public async ValueTask<List<Transaction>> SearchTransByDate(int partnerId, DateTime fromDate, DateTime toDate)
        {
            List<Transaction> listTransaction = new List<Transaction>();
            IQueryable<Transaction> rawData = _dbSet.Where(t => t.PartnerId == partnerId);
            if (fromDate.Date == toDate.Date)
            {
                var date = Convert.ToDateTime(fromDate).Date;
                var nextDay = date.AddDays(1);
                rawData = rawData.Where(t => date <= t.CreatedDate && t.CreatedDate < nextDay);
            }
            else
            {
                rawData = rawData.Where(t => fromDate.Date <= t.CreatedDate && t.CreatedDate <= toDate.Date);
            }
            listTransaction = await rawData.OrderByDescending(t => t.CreatedDate).ToListAsync();
            return listTransaction;
        }
        /*private Pagination<TopPartner> DoFilterTop10(PaginationParam paging, TransactionFilter filter, IQueryable<Transaction> queryable)
        {
            if (filter != null)
            {
                if ((DateTime.TryParse(filter.DateFrom, out DateTime dateFrom) && (DateTime.TryParse(filter.DateTo, out DateTime dateTo))))
                {
                    DateTime fromDate = DateTime.Parse(filter.DateFrom);
                    DateTime toDate = DateTime.Parse(filter.DateTo);
                    queryable = queryable.Where(p => p.CreatedDate.Date > fromDate && p.CreatedDate.Date < toDate);
                }
                if (DateTime.TryParse(filter.DateCreate, out DateTime date))
                {
                    DateTime transDate = DateTime.Parse(filter.DateCreate);
                    queryable = queryable.Where(p => p.CreatedDate.Date == transDate.Date);
                }
                if (Enum.TryParse(filter.TransactionType, out TransactionType transactionType))
                {
                    TransactionType type = (TransactionType)Enum.Parse(typeof(TransactionType), filter.TransactionType);
                    queryable = queryable.Where(p => p.TransactionType == type);
                }
                if (filter.PartnerName != null && filter.PartnerName.Length > 0)
                {
                    queryable = queryable.Where(p => p.Partner.DisplayName.Contains(filter.PartnerName));
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
            var query = queryable.GroupBy(c => c.PartnerId, (k, g) => new
            {
                id = k,
                totalWeight = filter.TransactionType.Equals("Import") ? g.Sum(b => b.WeightIn) - g.Sum(b => b.WeightOut) : g.Sum(b => b.WeightOut) - g.Sum(b => b.WeightIn)
            }); ; ;
            query = query.OrderByDescending(p => p.totalWeight);
            query = query.Skip((paging.Page - 1) * paging.Size).Take(paging.Size);
            Pagination<TopPartner> pagination = new Pagination<TopPartner>();
            pagination.Page = paging.Page;
            pagination.Size = paging.Size;
            double totalPage = (count * 1.0) / (pagination.Size * 1.0);
            List<TopPartner> listTop = new List<TopPartner>();
            PartnerRepository repo = new PartnerRepository();
            foreach (var item in query.ToList())
            {
                TopPartner top = new TopPartner();
                top.partner = repo.GetByID(item.id);
                top.totalWeight = item.totalWeight;
                listTop.Add(top);
            }
            pagination.TotalPage = (int)Math.Ceiling(totalPage);
            pagination.Data = listTop;

            return pagination;
        }*/
        public async ValueTask<Pagination<Transaction>> GetTransByPartnerIdAsync(PaginationParam paging, int id)
        {
            Pagination<Transaction> listTransaction = new Pagination<Transaction>();
            IQueryable<Transaction> rawData = null;
            rawData = _dbSet.Where(t => t.PartnerId == id);
            listTransaction = await DoPaging(paging, rawData);
            return listTransaction;

        }
        public async ValueTask<List<Transaction>> GetTransOfPartnerByDate(int partnerId, DateTime searchDate, string transactionStatus)
        {
            List<Transaction> listTransaction = new List<Transaction>();
            IQueryable<Transaction> rawData = null;
            var date = Convert.ToDateTime(searchDate).Date;
            var nextDay = date.AddDays(1);
            if (transactionStatus != null)
            {
                TransactionStatus trans;
                if (Enum.TryParse(transactionStatus, out trans))
                    rawData = _dbSet.Where(t => t.TransactionStatus == trans);
            }
            rawData = rawData.Where(t => t.PartnerId == partnerId && date <= t.CreatedDate && t.CreatedDate < nextDay);
            listTransaction = await rawData.OrderByDescending(t => t.CreatedDate).ToListAsync();
            return listTransaction;
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
            Transaction existed = _dbSet.OrderBy(t => t.TransactionId).Where(t => t.IdentificationCode.Equals(cardId) && t.TransactionStatus.Equals(TransactionStatus.Progessing)).LastOrDefault();
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
        public async Task<String> UpdateTransactionByManual(Transaction trans, int id)
        {
            if (id != trans.TransactionId)
            {
                return null;
            }
            DateTime current = DateTime.Now.Date;
            if (trans.CreatedDate <= current && trans.TimeIn <= current && trans.TimeOut <= current)
            {
                Transaction beforeTransaction = _dbSet.Find(id);
                if (beforeTransaction.PartnerId != trans.PartnerId)
                {
                    //change inventory detail
                    //get inventordy today
                    InventoryRepository inventRepo = new InventoryRepository();
                    Inventory inventory = await inventRepo.CheckExistDateRecord(trans.CreatedDate);
                    if (inventory != null)
                    {
                        InventoryDetailRepository detailRepo = new InventoryDetailRepository();
                        InventoryDetail lastPartner = await detailRepo.CheckExistedDetail(beforeTransaction.PartnerId, inventory.InventoryId);
                        InventoryDetail newPartner = await detailRepo.CheckExistedDetail(trans.PartnerId, inventory.InventoryId);
                        if (newPartner == null)
                        {
                            //tạo detail với số kí mới
                            await detailRepo.AddNewInventoryDetail(trans, inventory);
                        }
                        else
                        {
                            //update số kg
                            await detailRepo.UpdateInventoryDetailByType(trans, newPartner);
                        }
                        await detailRepo.UpdateInventoryWeight(beforeTransaction, lastPartner);
                    }
                }
                Update(trans);
                try
                {
                    await SaveAsync();
                    return "";
                }
                catch (Exception)
                {
                    return "Update failed";
                }
            }
            return "Time is not valid";
        }
        public async Task<Transaction> UpdateTransactionArduino(String cardId, float weightOut, int partnerId)
        {
            Partner partner = null;
            if (cardId != null && cardId.Length > 0) //update by nfc
            {
                //find provider and check card status
                IdentificationCodeRepository cardRepo = new IdentificationCodeRepository();
                Task<IdentityCard> checkCard = cardRepo.checkCard(cardId);
                if (checkCard.Result == null)
                {
                    //card not available
                    return null;
                }
                partner = cardRepo.GetPartnerCard(checkCard.Result.PartnerId).Result;
            }
            else if (partnerId != 0)
            {
                partner = _dbContext.Partner.Find(partnerId);
                if (partner != null && partner.PartnerStatus.Equals(PartnerStatus.Block)) return null;
            }
            //get partner failed
            if (partner == null)
            {
                return null;
            }


            bool checkProcessingCard = false;
            if (cardId != null && cardId.Length != 0)
            {
                checkProcessingCard = await CheckProcessingCard(cardId, "Update");
            }

            if (checkProcessingCard)
            {
                return null;
            }
            else
            {
                if (weightOut <= 0) return null;
                var trans = FindTransToWeightOut(cardId);
                if (trans != null)
                {
                    if (trans.WeightIn <= 0) return null;
                    float totalweight = 0;
                    if (trans.TransactionType.Equals(TransactionType.Import))
                    {
                        totalweight = trans.WeightIn - weightOut;
                        if (totalweight < 0) return null;
                    }
                    else if (trans.TransactionType.Equals(TransactionType.Export))
                    {
                        totalweight = weightOut - trans.WeightIn;
                        if (totalweight < 0) return null;
                    }
                    //set time out
                    trans.TimeOut = DateTime.Now;
                    //set weight out
                    trans.WeightOut = Rounding(weightOut, trans.TransactionType);
                    //change status
                    trans.TransactionStatus = TransactionStatus.Success;
                    if (trans.IdentificationCode == null || trans.IdentificationCode.Length == 0) trans.Description = "Forget identity card";
                    //update transaction
                    Update(trans);
                    try
                    {
                        await SaveAsync();
                        //update transaction thành công => tạo inventory detail
                        bool updateDetail = await UpdateInventoryDetail(trans);
                        if (updateDetail)
                        {
                            return trans;
                        }
                        return null;
                    }
                    catch (Exception)
                    {
                        if (GetByID(trans.TransactionId) == null)
                        {
                            return null;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        //tao inventory detail
        private async Task<Boolean> UpdateInventoryDetail(Transaction trans)
        {
            InventoryDetailRepository detailRepo = new InventoryDetailRepository();
            bool check = await detailRepo.UpdateInventoryDetail(trans.CreatedDate, trans);
            return check;
        }
        public async Task<string> CreateTransactionByManualAsync(Transaction trans)
        {
            //check partner
            Partner partner = null;
            if (trans.PartnerId != 0)
            {
                float totalWeight = 0;
                partner = _dbContext.Partner.Find(trans.PartnerId);
                if (partner != null && partner.PartnerStatus.Equals(PartnerStatus.Block)) return "Partner is not available";
                DateTime currentTime = DateTime.Now.AddDays(1).Date;
                if (trans.WeightIn <= 0) return "Weight in must greater than 0";
                if (trans.WeightOut <= 0) return "Weight out must greater than 0";
                if (trans.TimeIn > currentTime) return "Time is not available";
                if (trans.TimeOut > currentTime) return "Time is not available";
                if (trans.TransactionType.Equals(TransactionType.Export))
                {
                    totalWeight = trans.WeightOut - trans.WeightIn;
                    if (totalWeight < 0) return "The weight does not match the transaction type";
                }
                if (trans.TransactionType.Equals(TransactionType.Import))
                {
                    totalWeight = trans.WeightIn - trans.WeightOut;
                    if (totalWeight < 0) return "The weight does not match the transaction type";
                }
                if (trans.Description == null || trans.Description.Length == 0) return "Create transaction by manual must have a reason";
                trans.CreatedDate = DateTime.Now;
                trans.TransactionId = 0;
                trans.TransactionStatus = TransactionStatus.Success;
                Insert(trans);
                await SaveAsync();
                bool updateDetail = await UpdateInventoryDetail(trans);
                if (updateDetail)
                {
                    await UpdateMiscellaneousAsync(trans);
                    return "";
                }
                return "Can not update inventory";
            }
            else
            {
                return "Partner is no longer existed";
            }
        }
        //tạo transaction
        public async Task<String> CreateTransaction(Transaction trans)
        {
            //check card and provider
            Partner partner = null;
            if (trans.PartnerId != 0)
            {
                //insert by android card
                partner = _dbContext.Partner.Find(trans.PartnerId);
                if (partner != null && partner.PartnerStatus.Equals(PartnerStatus.Block)) return "Partner is not available";
                trans.Description = "Lost card";
            }
            if (trans.IdentificationCode != null && trans.IdentificationCode.Length != 0)
            {
                //insert by nfc

                //find provider and check card status
                IdentificationCodeRepository cardRepo = new IdentificationCodeRepository();

                //check valid card
                Task<IdentityCard> checkCard = cardRepo.checkCard(trans.IdentificationCode);
                if (checkCard.Result == null)
                {
                    //card not available
                    return "Card is not available";
                }
                partner = cardRepo.GetPartnerCard(checkCard.Result.PartnerId).Result;
            }

            //get partner failed
            if (partner == null)
            {
                return "Partner is not available";
            }


            bool checkSchedule = await CheckTransactionScheduled(trans.PartnerId);
            trans.IsScheduled = checkSchedule;
            //check validate weight in weight out
            if (trans.WeightIn <= 0)
            {
                return "Weight in must be greater than 0";
            }
            if (trans.WeightOut <= 0) return "Weight in must be greater than 0";

            //disable transaction before
            if (trans.IdentificationCode != null && trans.IdentificationCode.Length != 0)
            {
                bool checkProceesingCard = await CheckProcessingCard(trans.IdentificationCode, "Insert");
                if (checkProceesingCard) return null;
            }

            //check type
            if (partner.PartnerTypeId == 1) trans.TransactionType = TransactionType.Export;
            if (partner.PartnerTypeId == 2) trans.TransactionType = TransactionType.Import;
            float totalweight = 0;
            if (trans.TransactionType.Equals(TransactionType.Import))
            {
                totalweight = trans.WeightIn - trans.WeightOut;
                if (totalweight < 0) return "The weight does not match the transaction type";
            }
            else if (trans.TransactionType.Equals(TransactionType.Export))
            {
                totalweight = trans.WeightOut - trans.WeightIn;
                totalweight = trans.WeightOut - trans.WeightIn;
                if (totalweight < 0) return "The weight does not match the transaction type";
            }

            //check hợp lệ => tạo transaction
            trans.TransactionId = 0;
            trans.PartnerId = partner.PartnerId;
            trans.GoodsId = _dbContext.Goods.First().GoodsId;
            trans.WeightIn = Rounding(trans.WeightIn, trans.TransactionType);
            trans.WeightOut = Rounding(trans.WeightOut, trans.TransactionType);
            trans.TransactionStatus = TransactionStatus.Success;
            bool updateDetail = await UpdateInventoryDetail(trans);
            if (updateDetail)
            {
                Insert(trans);
                return "";
            }
            return "Update detail inventory failed";
        }

        //check transaction is scheduled or not
        public async Task<bool> CheckTransactionScheduled(int partnerId)
        {
            bool check = false;
            ScheduleRepository scheduleRepo = new ScheduleRepository();
            //get list schedule that partner is booked in date
            List<Schedule> listBookedSchedule = await scheduleRepo.GetBookedScheduleInDate(partnerId);
            if (listBookedSchedule != null && listBookedSchedule.Count != 0)
            {
                //partner co datlich
                check = true;
            }
            else
            {
                check = false;
            }
            return check;
        }
        public async Task<string> UpdateMiscellaneousAsync(Transaction transaction)
        {

            float totalWeight = transaction.WeightIn - transaction.WeightOut;
            if (totalWeight < 0) totalWeight = totalWeight * -1;

            GoodsRepository goodsRepository = new GoodsRepository();
            goodsRepository.UpdateQuantityOfGood(transaction.GoodsId, totalWeight, transaction.TransactionType);

            String check = "";
            bool checkUpdateSchedule = false;
            //update schedule
            if (transaction.IsScheduled)
            {
                ScheduleRepository scheduleRepo = new ScheduleRepository();
                checkUpdateSchedule = await scheduleRepo.UpdateActualWeight(transaction.PartnerId, totalWeight);
                if (!checkUpdateSchedule)
                {
                    check = "Weight is not valid with register weight";
                }
            }
            if (!transaction.IsScheduled || !checkUpdateSchedule)
            {
                //transaction chưa đặt lịch
                TimeTemplateItemRepository timeTemplateItemRepository = new TimeTemplateItemRepository();
                List<TimeTemplateItem> listItem = await timeTemplateItemRepository.GetAppliedItem();
                TimeSpan timeOut = TimeSpan.Parse(transaction.TimeOut.ToString("HH:mm"));
                TimeTemplateItem timeItem = null;
                foreach (var item in listItem)
                {
                    if (timeOut > item.ScheduleTime)
                    {
                        timeItem = item;
                        break;
                    }
                }
                if (timeItem == null)
                {
                    if (listItem.First().ScheduleTime > timeOut) timeItem = listItem.First();
                    if (listItem.Last().ScheduleTime < timeOut) timeItem = listItem.Last();

                }
                timeTemplateItemRepository.UpdateCurrent(transaction.TransactionType, totalWeight, timeItem.TimeTemplateItemId);
            }
            return check;
        }

        public async void CancelProcessing()
        {
            List<Transaction> transactions = await _dbSet.Where(t => t.TransactionStatus == TransactionStatus.Progessing).ToListAsync();
            foreach (var item in transactions)
            {
                item.TransactionStatus = TransactionStatus.Disable;
                item.TimeOut = DateTime.Now;
                item.Description = "Disable " + SystemName.System.ToString();
                _dbContext.Entry(item).State = EntityState.Modified;
            }
            await SaveAsync();
        }
        private float Rounding(float weight, TransactionType type)
        {
            return (float)Math.Round(weight, 2);
        }

        public async Task<String> GetStatictis(string username, int partnerId, DateTime fromDate, DateTime toDate, SmtpSetting server, FirebaseSetting firebase)
        {
            Account account = _dbContext.Account.Find(username);
            Partner partner = _dbContext.Partner.Find(partnerId);
            if (partner != null)
            {
                if (account.RoleId == 3 && !account.Username.Equals(partner.Username))
                {
                    return "Account invalid to request report";
                }
                else
                {
                    List<Transaction> listTransaction = await SearchTransByDate(partnerId, fromDate, toDate);
                    if (listTransaction != null)
                    {
                        float totalWeight = 0;
                        float temp = 0;
                        foreach (var item in listTransaction)
                        {
                            temp = item.WeightIn - item.WeightOut;
                            if (temp < 0) temp = temp * -1;
                            totalWeight += temp;
                        }
                        totalWeight = (float) Math.Round(totalWeight, 2);
                        String date = "";
                        if (fromDate.Date != toDate.Date)
                        {
                            date = fromDate.ToString("dd/MM/yyyy") + " - " + toDate.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            date = fromDate.ToString("dd/MM/yyyy");
                        }
                        if (listTransaction.Count == 0)
                        {
                            MessageSetting mail = new MessageSetting(partner.DisplayName, username, date, listTransaction.Count, totalWeight, "No transactions have been made at this time");
                            SendEmail(server, mail, partner);
                            return "";
                        }
                        else
                        {
                            string path = Environment.SpecialFolder.UserProfile + @"\Downloads";
                            string filename = Regex.Replace(partner.DisplayName, @"\s+", "") + DateTime.Now.ToString("ddMMyyyy") + DateTime.Now.ToString("hhmmss") + ".xlsx";
                            string filePath = path + "\\" + filename;
                            bool checkExport = ExportExcel(listTransaction, filePath);
                            if (checkExport)
                            {
                                FirebaseRepository firebaseRepo = new FirebaseRepository();
                                String downloadUrl = await firebaseRepo.GetFile(firebase, filename, filePath);
                                MessageSetting mail = new MessageSetting(partner.DisplayName, username, date, listTransaction.Count, totalWeight, downloadUrl);
                                bool check = SendEmail(server, mail, partner);
                                if(check){
                                    return "";
                                }else
                                {
                                    return "Send mail failed";
                                }
                            }
                            else
                            {
                                return "Export report excel failed";
                            }
                        }
                    }
                    else
                    {
                        return "Get transaction failed";
                    }
                }
            }
            else
            {
                return "Partner no longer exist";
            }
        }
        static DataTable ConvertToDataTable<T>(List<T> models)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names    
                dataTable.Columns.Add(prop.Name);
            }
            // Adding Row
            foreach (T item in models)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                // Finally add value to datatable    
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
        private bool ExportExcel(List<Transaction> listTransaction, String filePath)
        {
            try
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    workbook.Worksheets.Add(ConvertToDataTable<Transaction>(listTransaction), "Transaction");
                    workbook.SaveAs(filePath);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        private bool SendEmail(SmtpSetting serverEmail, MessageSetting mailContent, Partner partner)
        {
            bool check = true;
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("baokhanh.buido@gmail.com", "ICAN Automatic Mailer ", System.Text.Encoding.UTF8);
                    mail.To.Add("khanhbdbse130392@fpt.edu.vn");
                    mail.Subject = "Request Captcha";
                    mail.Body = "<h1>Your Captcha is " + "test" + "</h1>" +
                        "<h2>This Captcha will expire within 5 minutes</h2>";
                    mail.IsBodyHtml = true;
                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential("tanntse63184@fpt.edu.vn", "buidobaokhanh0711");
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Send(mail);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
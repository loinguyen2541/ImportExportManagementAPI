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
    public class IdentificationCodeRepository : BaseRepository<IdentityCard>
    {
        public async Task<Pagination<IdentityCard>> GetAllAsync(PaginationParam paging, IdentityCardFilter filter)
        {
            Pagination<IdentityCard> IdentityCards = new Pagination<IdentityCard>();
            IQueryable<IdentityCard> rawData = null;
            rawData = _dbSet.Include(p => p.Partner);
            IdentityCards = await DoFilterAsync(paging, filter, rawData);
            //schedules = _dbSet.ToList();
            return IdentityCards;
        }

        public async Task<List<IdentityCard>> GetListPending()
        {
            List<IdentityCard> IdentityCards = new List<IdentityCard>();
            IQueryable<IdentityCard> rawData = null;
            rawData = _dbSet.Include(p => p.Partner).Where(i => i.IdentityCardStatus.Equals(IdentityCardStatus.Pending));
            IdentityCards = await rawData.ToListAsync();
            return IdentityCards;
        }

        private async Task<Pagination<IdentityCard>> DoFilterAsync(PaginationParam paging, IdentityCardFilter filter, IQueryable<IdentityCard> queryable)
        {
            if (filter.PartnerName != null && filter.PartnerName.Length > 0)
            {
                queryable = queryable.Where(p => p.Partner.DisplayName.Contains(filter.PartnerName));
            }

            if (Enum.TryParse(filter.Status, out IdentityCardStatus identityCardStatus))
            {
                queryable = queryable.Where(p => p.IdentityCardStatus == identityCardStatus);
            }
            if (filter.PartnerTypeId > 0)
            {
                queryable = queryable.Where(p => p.Partner.PartnerTypeId == filter.PartnerTypeId);
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
            Pagination<IdentityCard> pagination = new Pagination<IdentityCard>();
            pagination.Page = paging.Page;
            pagination.Size = paging.Size;
            double totalPage = (count * 1.0) / (pagination.Size * 1.0);
            pagination.TotalPage = (int)Math.Ceiling(totalPage);
            pagination.Data = await queryable.ToListAsync();
            return pagination;
        }

        public List<IdentityCard> GetIdentityCards()
        {
            List<IdentityCard> IdentityCards = null;
            IdentityCards = _dbSet.Where(p => p.IdentityCardStatus == IdentityCardStatus.Active).ToList();
            return IdentityCards;
        }
        public bool Exist(String id)
        {
            return _dbSet.Any(e => e.IdentityCardId.Equals(id));
        }

        public void DeleteIdentityCard(IdentityCard IdentityCard)
        {
            IdentityCard.IdentityCardStatus = IdentityCardStatus.Block;
            Update(IdentityCard);
        }

        //lay provider cua card
        public async Task<Partner> GetPartnerCard(int partnerId)
        {
            PartnerRepository partnerRepo = new PartnerRepository();
            var partner = await partnerRepo.GetByIDAsync(partnerId);
            if (partner == null || partner.PartnerStatus.Equals(PartnerStatus.Block))
            {
                return null;
            }
            return partner;
        }

        //check scand card
        public async Task<IdentityCard> checkCard(String cardId)
        {
            if (cardId != null)
            {
                //card có nằm trong hệ thống không
                var identityCard = await GetByIDAsync(cardId);
                if (identityCard != null && identityCard.IdentityCardStatus.Equals(IdentityCardStatus.Active))
                {
                    return identityCard;
                }
            }
            return null;
        }
        public List<IdentityCardStatus> GetCardsStatus()
        {
            return Enum.GetValues(typeof(IdentityCardStatus)).Cast<IdentityCardStatus>().ToList();

        }
    }
}
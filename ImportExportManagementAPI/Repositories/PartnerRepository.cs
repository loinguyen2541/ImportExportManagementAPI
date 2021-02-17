using ImportExportManagement_API.Models;
using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagement_API.Repositories
{
    public class PartnerRepository : BaseRepository<Partner>
    {
        //get all partner has status active
        //auto load for transaction create in destop application
        public async ValueTask<List<Partner>> GetAllPartner(Paging paging)
        {
            List<Partner> listPartner = new List<Partner>();
            IQueryable<Partner> rawData = null;
            rawData = rawData.Where(p => p.PartnerStatus == PartnerStatus.Active);
            listPartner = await rawData.ToListAsync();
            return listPartner;
        }
    }
}

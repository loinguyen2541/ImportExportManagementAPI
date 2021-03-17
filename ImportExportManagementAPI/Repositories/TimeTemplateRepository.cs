using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

/**
* @author Loi Nguyen
*
* @date - 3/11/2021 12:20:01 PM 
*/

namespace ImportExportManagementAPI.Repositories
{
    public class TimeTemplateRepository : BaseRepository<TimeTemplate>
    {
        public async void ResetSchedule(float inventory)
        {

            TimeTemplate timeTemplateApplied = await _dbSet.Include(t => t.TimeTemplateItems)
                .Where(p => p.TimeTemplateStatus == TimeTemplateStatus.Applied).SingleOrDefaultAsync();

            TimeTemplate timeTemplatePending = await _dbSet.Include(t => t.TimeTemplateItems)
                .Where(p => p.TimeTemplateStatus == TimeTemplateStatus.Pending).SingleOrDefaultAsync();

            if (timeTemplatePending == null)
            {
                //Update current applied template
                UpdateInventory(timeTemplateApplied, inventory);
                _dbContext.Entry(timeTemplateApplied).State = EntityState.Modified;
            }
            else
            {
                //Change current pending template to applied and update it's items
                timeTemplatePending.TimeTemplateStatus = TimeTemplateStatus.Applied;
                UpdateInventory(timeTemplatePending, inventory);
                _dbContext.Entry(timeTemplatePending).State = EntityState.Modified;

                //Disable current applied template
                if (timeTemplateApplied != null)
                {
                    timeTemplateApplied.TimeTemplateStatus = TimeTemplateStatus.Disabled;
                    _dbContext.Entry(timeTemplateApplied).State = EntityState.Modified;
                }
            }

            await SaveAsync();
        }

        private void UpdateInventory(TimeTemplate timeTemplate, float inventory)
        {
            foreach (var item in timeTemplate.TimeTemplateItems)
            {
                item.Inventory = inventory;
            }
        }

        public async Task<TimeTemplate> GetCurrentTimeTemplateAsync(int partnerId)
        {
            TimeTemplate timeTemplate = await _dbSet.
                Where(t => t.TimeTemplateStatus == TimeTemplateStatus.Applied)
                .Include(t => t.TimeTemplateItems).ThenInclude(t => t.Schedules.Where(p => p.PartnerId == partnerId && p.IsCanceled == false))
                .SingleOrDefaultAsync();
            return timeTemplate;
        }
    }
}

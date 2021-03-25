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
        /// <summary>
        ///  This method resets a applied time template base on current inventory. <br/>
        ///  If there is a peding item template in the system, then update the pending status of that object to be applied 
        ///  and change status of applied item template to disable.
        /// </summary>
        /// <param name="inventory"></param>
        public async void ResetTimeTemplate(float inventory)
        {

            TimeTemplate timeTemplateApplied = await _dbSet.Include(t => t.TimeTemplateItems)
                .Where(p => p.TimeTemplateStatus == TimeTemplateStatus.Applied).SingleOrDefaultAsync();
            await _dbContext.Entry(timeTemplateApplied).ReloadAsync();

            TimeTemplate timeTemplatePending = await _dbSet.Include(t => t.TimeTemplateItems)
                .Where(p => p.TimeTemplateStatus == TimeTemplateStatus.Pending).SingleOrDefaultAsync();

            if (timeTemplatePending == null)
            {
                //Update current applied template
                UpdateCurrentApliedTemplate(timeTemplateApplied, inventory);

            }
            else
            {
                if (timeTemplatePending.ApplyingDate <= DateTime.Now.Date)
                {
                    //Change current pending template to applied and update it's items
                    timeTemplatePending.TimeTemplateStatus = TimeTemplateStatus.Applied;
                    timeTemplatePending.ApplyingDate = DateTime.Now.Date.AddDays(1);
                    UpdateInventory(timeTemplatePending, inventory);
                    _dbContext.Entry(timeTemplatePending).State = EntityState.Modified;

                    //Disable current applied template
                    if (timeTemplateApplied != null)
                    {
                        timeTemplateApplied.TimeTemplateStatus = TimeTemplateStatus.Disabled;
                        _dbContext.Entry(timeTemplateApplied).State = EntityState.Modified;
                    }
                }
                else
                {
                    //Update current applied template
                    UpdateCurrentApliedTemplate(timeTemplateApplied, inventory);
                }
            }

            await SaveAsync();
        }

        private void UpdateCurrentApliedTemplate(TimeTemplate timeTemplate, float inventory)
        {
            //Update current applied template
            if (timeTemplate.ApplyingDate.Date <= DateTime.Now.Date)
            {
                timeTemplate.ApplyingDate = DateTime.Now.Date.AddDays(1);
                UpdateInventory(timeTemplate, inventory);
                _dbContext.Entry(timeTemplate).State = EntityState.Modified;
            }
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
        public async Task<TimeTemplate> GetForecastInventoryToday()
        {
            TimeTemplate timeTemplate = await _dbSet.Where(t => t.ApplyingDate == DateTime.Today && t.TimeTemplateStatus == TimeTemplateStatus.Applied)
                .Include(t => t.TimeTemplateItems.Where( i => i.ScheduleTime > DateTime.Now.TimeOfDay)).SingleOrDefaultAsync();
            return timeTemplate;
        }
        public TimeTemplate GetCurrentTimeTemplate()
        {
            TimeTemplate timeTemplate = _dbSet.AsNoTracking().
                Where(t => t.TimeTemplateStatus == TimeTemplateStatus.Applied).SingleOrDefault();
            return timeTemplate;
        }

    }
}

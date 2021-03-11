using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/**
* @author Loi Nguyen
*
* @date - 3/11/2021 11:01:49 AM 
*/

namespace ImportExportManagementAPI.Models
{
    public class TimeTemplate
    {
        public int TimeTemplateId { get; set; }
        public int TimeTemplateName { get; set; }
        public TimeTemplateStatus TimeTemplateStatus { get; set; }
        public List<TimeTemplateItem> TimeTemplateItems { get; set; }
    }
}

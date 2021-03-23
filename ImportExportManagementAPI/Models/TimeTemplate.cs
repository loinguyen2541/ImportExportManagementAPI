using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [MaxLength(25)]
        public String TimeTemplateName { get; set; }

        [Column(TypeName = "Date")]
        public DateTime ApplyingDate { get; set; }

        public TimeTemplateStatus TimeTemplateStatus { get; set; }

        public List<TimeTemplateItem> TimeTemplateItems { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/**
* @author Loi Nguyen
*
* @date - 2/16/2021 2:29:41 AM 
*/

namespace ImportExportManagementAPI.Models
{
    public class Pagination<TEntity> where TEntity : class
    {
        public int Size { get; set; }
        public int Page { get; set; }
        public int TotalPage { get; set; }
        public List<TEntity> Data { get; set; }
    }
}

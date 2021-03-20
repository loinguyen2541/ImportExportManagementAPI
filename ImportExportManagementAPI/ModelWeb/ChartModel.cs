using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.ModelWeb
{
    public class ChartModel
    {
        public List<int> Data { get; set; }
        public List<int> Data2 { get; set; }
        public List<String> Label { get; set; }
        public ChartModel()
        {
            this.Data = new List<int>();
            this.Data2 = new List<int>();
            this.Label = new List<String>();
        }
    }
}

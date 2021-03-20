using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.ModelWeb;
using ImportExportManagementAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.DataStore
{
    public static class DataManager
    {
        private static InventoryRepository inventoryRepository = new InventoryRepository();

        public static ChartModel GetData()
        {   
            DateTime now = DateTime.Now;
            List<TotalInventoryDetailedByDate> listDetailed = inventoryRepository.TotalWeightInventoryFloatByMonth(now.AddDays(-7),now);
            List<string> listLabel = new List<string>();
            List<int> listImport = new List<int>();
            List<int> listExport = new List<int>();
            foreach (var item in listDetailed)
            {
                listLabel.Add(String.Format("{0}-{1}",item.date.Day,item.date.Month));
            }
            foreach (var item in listDetailed)
            {
                string date = String.Format("{0}-{1}", item.date.Day, item.date.Month);
                foreach (var label in listLabel)
                {
                    if (date.Equals(label))
                    {
                        if(item.type == InventoryDetailType.Import)
                        {
                            listImport.Add(Int32.Parse(item.weight.ToString()));
                        }
                        if (item.type == InventoryDetailType.Export)
                        {
                            listExport.Add(Int32.Parse(item.weight.ToString()));
                        }
                    }

                }
            }
            return new ChartModel {
                Data = listImport,
                Data2 = listExport,
                Label = listLabel
            };
        }
    }
}

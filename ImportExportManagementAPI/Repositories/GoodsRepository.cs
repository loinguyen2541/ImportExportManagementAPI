using ImportExportManagement_API.Models;
using ImportExportManagement_API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Repositories
{
    public class GoodsRepository : BaseRepository<Goods>
    {
        public List<Goods> GetGoods()
        {
            List<Goods> goods = null;
            goods = _dbSet.ToList();
            return goods;
        }
        public bool Exist(int id)
        {
            return _dbSet.Any(e => e.GoodsId == id);
        }

        public void DeleteGoods(Goods  goods)
        {
           // partner.PartnerStatus = PartnerStatus.Block;
            Update(goods);
        }
    }
}

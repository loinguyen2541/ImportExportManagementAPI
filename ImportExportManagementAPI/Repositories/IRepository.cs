using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


/**
* @author Loi Nguyen
*
* @date - 1/30/2021 11:16:05 PM 
*/

namespace ImportExportManagement_API.Repositories
{
    interface IRepository<TEntity> where TEntity : class
    {
        List<TEntity> GetAll();
        TEntity GetByID(object id);
        void Delete(TEntity entityToDelete);
        void Delete(object id);
        void Insert(TEntity entity);
        void Update(TEntity entityToUpdate);
    }
}

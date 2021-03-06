using ImportExportManagement_API.Models;
using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Repositories
{
    public class AccountRepository : BaseRepository<Account>
    {
        public List<Account> GetAccounts(AccountStatus status)
        {
            List<Account> accounts = null;
            accounts = _dbSet.Where(p=> p.Status == status).ToList();
            return accounts;
        }
        public string checkLogin(Account account)
        {
            IQueryable<Account> username = 
                _dbContext.Set<Account>().FromSqlRaw
                (String.Format("EXEC CheckLogin @username = '{0}', @password = '{1}'",account.Username,account.Password));
            if(username != null)
            {
                List<Account> accounts = username.ToList();
               return accounts.First().Username;
            }
            return null;
        }

        public async Task<Account> GetAccount(String username)
        {
           return await _dbSet.Include(p => p.Partner).Where(a => a.Username.Equals(username)).FirstAsync();
        }

        public bool Exist(string username)
        {
            return _dbSet.Any(e => e.Username.Equals(username));
        }

        internal void DeleteAccount(Account account)
        {
            account.Status = AccountStatus.Block;
            Update(account);
        }
    }
}

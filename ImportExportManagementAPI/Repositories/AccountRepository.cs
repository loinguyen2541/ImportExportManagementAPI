using ImportExportManagement_API.Models;
using ImportExportManagement_API.Repositories;
using ImportExportManagementAPI.Helper;
using ImportExportManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Repositories
{
    public class AccountRepository : BaseRepository<Account>
    {
        public List<Account> GetAccounts(AccountStatus status)
        {
            List<Account> accounts = null;
            accounts = _dbSet.Where(p => p.Status == status).ToList();
            return accounts;
        }
        public string checkLogin(Account account)
        {
            IQueryable<Account> username =
                _dbContext.Set<Account>().FromSqlRaw
                (String.Format("EXEC CheckLogin @username = '{0}', @password = '{1}'", account.Username, account.Password));
            if (username != null)
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

        public async Task<List<Account>> GetAccountByRole(int roleId)
        {
            List<Account> accounts = await _dbSet.Include(p => p.Partner).Include(r => r.Role).Where(a => a.RoleId == roleId).ToListAsync();       
            return accounts;
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
        public async Task<Account> Login(AuthenticateModel model, AppSettings _appSettings)
        {
            Account account = GetByID(model.Username);
            if (account != null)
            {
                if (account.Password.Equals(model.Password) && account.Status.Equals(AccountStatus.Active))
                {
                    //authenticate success
                    if (account.Token == null || account.Token.Length == 0)
                    {
                        string role = "Manager";
                        if (account.RoleId == 2) role = "Staff";
                        else if (account.RoleId == 3) role = "Partner";
                        //generate token and return it
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                        var validIssuer = _appSettings.Issuer;
                        var validAudience = _appSettings.Audience;
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                    new Claim(ClaimTypes.Name, account.Username.ToString()),
                    new Claim(ClaimTypes.Role, role)
                            }),
                            Audience = validAudience,
                            Issuer = validIssuer,
                            Expires = DateTime.UtcNow.AddMinutes(30),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                        };
                        var token = tokenHandler.CreateToken(tokenDescriptor);
                        account.Token = tokenHandler.WriteToken(token);
                        try
                        {
                            Update(account);
                            await SaveAsync();
                        }
                        catch
                        {
                            return null;
                        }
                    }
                    return WithoutPassword(account);
                }
            }
            return null;
        }
    }
}

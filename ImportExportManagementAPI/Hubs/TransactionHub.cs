using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/**
* @author Loi Nguyen
*
* @date - 3/19/2021 4:27:34 PM 
*/

namespace ImportExportManagementAPI.Hubs
{
    public class TransactionHub : Hub
    {
        public async Task SendMessage()
        {
            await Clients.All.SendAsync("ReloadTransaction", "reload");
        }
    }
}

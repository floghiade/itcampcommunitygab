using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureWorkshop.Hubs
{
    public class Alerts : Hub
    {

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
    }
}

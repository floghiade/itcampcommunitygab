using AzureWorkshop.Models;
using ITCamp.Gab.Core;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureWorkshop.Hubs
{
    public class AlertsHubManager
    {
        public IHubContext<Alerts> _alertsHub;

        public AlertsHubManager(IHubContext<Alerts> alertHubContext)
        {
            _alertsHub = alertHubContext;
        }

        public async Task SendAlert(WeatherAlert alert)
        {
            await _alertsHub.Clients.All.SendAsync("SendAlert", new object[] { AlertModel.FromCore(alert) });
        }
    }
}

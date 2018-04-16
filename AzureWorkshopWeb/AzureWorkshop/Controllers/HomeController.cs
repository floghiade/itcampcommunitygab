using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AzureWorkshop.Models;
using Microsoft.AspNetCore.SignalR;
using AzureWorkshop.Hubs;
using Microsoft.Extensions.Options;

namespace AzureWorkshop.Controllers
{
    public class HomeController : Controller
    {
        private IHubContext<Alerts> _alertHubContext;
        private CountrySettings _settings;

        public HomeController(IHubContext<Alerts> alertHubContext, IOptions<CountrySettings> settings)
        {
            _alertHubContext = alertHubContext;
            _settings = settings.Value;
        }

        public IActionResult Index()
        {
            RedisDb cache = new RedisDb();
            var alerts = cache.GetFromCache(_settings.Code);
            ViewData["Country"] = _settings.Name;

            return View(alerts.Select(x => AlertModel.FromCore(x)));

            //List<AlertModel> data = new List<AlertModel>
            //{
            //    new AlertModel { Message = "This is the first alert!"},
            //    new AlertModel { Message = "This is the second alert!"}
            //};
            //return View(data);
        }

        public IActionResult Error()
        {

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Trigger()
        {
            await _alertHubContext.Clients.All.SendAsync("SendAlert", "This is an alert message sent from the controller.");

            return Ok();
        }
    }
}

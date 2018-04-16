using ITCamp.Gab.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureWorkshop.Models
{
    public class AlertModel
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public static AlertModel FromCore(WeatherAlert alert)
        {
            return new AlertModel
            {
                Title = $"{alert.Level.ToString()} Alert",
                Message = $"{alert.AlertType} alert valid from {alert.ValidFrom} till {alert.ValidFrom + alert.Period}"
            };
        }
    }
}

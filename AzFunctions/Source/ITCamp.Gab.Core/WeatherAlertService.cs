using System;

namespace ITCamp.Gab.Core
{
    public class WeatherAlertService
    {
        public WeatherAlert GetWeatherAlert()
        {
            Random randomNumber = new Random();

            WeatherAlert alert = new WeatherAlert()
            {
                CountryCode = randomNumber.Next() % 2 == 0 ? "RO" : "BG",
                Level = (AlertLevel)randomNumber.Next(Enum.GetNames(typeof(AlertLevel)).Length),
                AlertType = (WeatherAlertType) randomNumber.Next(Enum.GetNames(typeof(WeatherAlertType)).Length),
                ValidFrom = DateTime.UtcNow,
                Period = TimeSpan.FromMinutes(randomNumber.Next(1,5))
            };

            return alert;
        }
    }
}

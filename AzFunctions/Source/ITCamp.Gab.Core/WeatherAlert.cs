using System;

namespace ITCamp.Gab.Core
{
    public class WeatherAlert
    {
        /// <example>
        /// RO  BG
        /// </example>
        public string CountryCode
        {
            get;
            set;
        }

        public WeatherAlertType AlertType
        {
            get;
            set;
        }

        public AlertLevel Level
        {
            get;
            set;
        }

        public DateTime ValidFrom
        {
            get;
            set;
        }

        public TimeSpan Period
        {
            get;
            set;
        }

        public override string ToString()
        {
            return $"Valid from: '{ValidFrom}' Pediod: '{Period.TotalMinutes}' Country Code: '{CountryCode}' Type: '{AlertType}' Level: '{Level}' ";
        }

    }
}

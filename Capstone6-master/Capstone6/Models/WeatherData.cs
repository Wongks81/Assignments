using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Capstone6.Models
{
    public class WeatherData
    {
        [JsonProperty("consolidated_weather")]
        public WeatherDataItem[] ConsolidatedWeather { get; set; }
    }

    public class WeatherDataItem
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("weather_state_name")]
        public string WeatherStateName { get; set; }

        [JsonProperty("min_temp")]
        public double MinTemp { get; set; }

        [JsonProperty("max_temp")]
        public double MaxTemp { get; set; }

        [JsonProperty("the_temp")]
        public double TheTemp { get; set; }
    }

    public class WeatherLocationData
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("woeid")]
        public long Woeid { get; set; }

        [JsonProperty("latt_long")]
        public string LattLong { get; set; }
    }
}

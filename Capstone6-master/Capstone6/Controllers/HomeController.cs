using Capstone6.Data;
using Capstone6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace Capstone6.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            SetWeatherData().Wait();
            ViewData["dCount"] = _context.Departments.Count();
            ViewData["sCount"] = _context.Subjects.Count();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task SetWeatherData()
        {
            double lon = 1.3521;
            double lang = 103.8198;
            try
            {
                var locationDataUrl = $"https://wedra.azurewebsites.net/api/location/search?lattlong={lang},{lon}";
                HttpClient client = new HttpClient();
                string locationDataResult = await client.GetStringAsync(locationDataUrl);
                var locationData = JsonConvert.DeserializeObject<WeatherLocationData>(locationDataResult);

                var locationWeatherDataUrl = $"https://wedra.azurewebsites.net/api/location/{locationData.Woeid}";
                string locationWeatherDataResult = await client.GetStringAsync(locationWeatherDataUrl);
                var locationWeatherData = JsonConvert.DeserializeObject<WeatherData>(locationWeatherDataResult);

                ViewData["weatherTitle"] = locationData.Title;
                ViewData["weatherStateName"] = locationWeatherData.ConsolidatedWeather[0].WeatherStateName;
                ViewData["weatherTemp"] = Math.Floor(locationWeatherData.ConsolidatedWeather[0].TheTemp);
                ViewData["maxTemp"] = Math.Floor(locationWeatherData.ConsolidatedWeather[0].MaxTemp);
                ViewData["minTemp"] = Math.Floor(locationWeatherData.ConsolidatedWeather[0].MinTemp);

            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }

       
    }
}

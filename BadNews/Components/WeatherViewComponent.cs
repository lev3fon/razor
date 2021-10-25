using System.Threading.Tasks;
using BadNews.Repositories.Weather;
using Microsoft.AspNetCore.Mvc;

namespace BadNews.Components
{
    public class WeatherViewComponent: ViewComponent
    {
        private readonly IWeatherForecastRepository repository;

        public WeatherViewComponent(IWeatherForecastRepository repository)
        {
            this.repository = repository;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var weatherForecast = await repository.GetWeatherForecastAsync();
            return View(weatherForecast);
        }
    }
}
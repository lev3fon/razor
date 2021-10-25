using System;
using System.Threading.Tasks;

namespace BadNews.Repositories.Weather
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private const string defaultWeatherImageUrl = "/images/cloudy.png";

        private readonly Random random = new Random();

        public async Task<WeatherForecast> GetWeatherForecastAsync()
        {
            return BuildRandomForecast();
        }

        private WeatherForecast BuildRandomForecast()
        {
            var temperature = random.Next(-20, 20 + 1);
            return new WeatherForecast
            {
                TemperatureInCelsius = temperature,
                IconUrl = defaultWeatherImageUrl
            };
        }
    }
}
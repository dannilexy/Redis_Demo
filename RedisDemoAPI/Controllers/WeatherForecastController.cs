using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RedisDemoAPI.Data.Extensions;

namespace RedisDemoAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly IDistributedCache _cache;
        private readonly string recordKey;
        public WeatherForecastController(IDistributedCache _cache)
        {
            this._cache = _cache;
            recordKey = "weatherForeCast_Test";
        }

        [HttpGet("SetWeatherForecast")]
        public async Task<IActionResult> SetWeatherForecast()
        {

            var weather = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
            await DistributedCacheExtension.SetRecordAsync(_cache, recordKey, weather);

            return Ok(weather);

        }

        [HttpGet("GetWeatherForecast")]
        public async Task<IActionResult> GetWeatherForeCast()
        {
            var weatherFromCache = await DistributedCacheExtension.GetRecordAsync<List<WeatherForecast2>>(_cache, recordKey);
            if (weatherFromCache is not null)
            {
                return Ok(weatherFromCache);

            }
            return BadRequest("Not content available to retreive");

        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartParkingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(IHttpClientFactory httpClientFactory, ILogger<WeatherController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        // GET: api/weather/Skopje
        // Third-party API integration using Open-Meteo geocoding and weather API
        [AllowAnonymous]
        [HttpGet("{city}")]
        public async Task<IActionResult> GetWeatherByCity(string city)
        {
            try
            {
                _logger.LogInformation("Getting weather information for city: {City}", city);

                var geoUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={city}&count=1";

                var geoResponse = await _httpClient.GetFromJsonAsync<GeocodingResponse>(geoUrl);

                if (geoResponse == null || geoResponse.Results == null || geoResponse.Results.Count == 0)
                {
                    _logger.LogWarning("City not found: {City}", city);
                    return NotFound("City not found.");
                }

                var location = geoResponse.Results[0];

                var weatherUrl =
                    $"https://api.open-meteo.com/v1/forecast?latitude={location.Latitude}&longitude={location.Longitude}&current_weather=true";

                var weatherResponse = await _httpClient.GetFromJsonAsync<WeatherResponse>(weatherUrl);

                if (weatherResponse == null || weatherResponse.CurrentWeather == null)
                {
                    _logger.LogWarning("Weather information not found for city: {City}", city);
                    return NotFound("Weather information not found.");
                }

                return Ok(new
                {
                    city = location.Name,
                    country = location.Country,
                    latitude = location.Latitude,
                    longitude = location.Longitude,
                    temperature = weatherResponse.CurrentWeather.Temperature,
                    windspeed = weatherResponse.CurrentWeather.Windspeed,
                    weatherCode = weatherResponse.CurrentWeather.Weathercode,
                    time = weatherResponse.CurrentWeather.Time
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting weather information for city: {City}", city);
                return StatusCode(500, "An error occurred while retrieving weather information.");
            }
        }
    }

    public class GeocodingResponse
    {
        public List<GeocodingResult>? Results { get; set; }
    }

    public class GeocodingResult
    {
        public string Name { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }

    public class WeatherResponse
    {
        public CurrentWeather? CurrentWeather { get; set; }
    }

    public class CurrentWeather
    {
        public double Temperature { get; set; }

        public double Windspeed { get; set; }

        public int Weathercode { get; set; }

        public string Time { get; set; } = string.Empty;
    }
}
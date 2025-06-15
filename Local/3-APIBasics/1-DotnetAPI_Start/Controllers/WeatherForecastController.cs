// see https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-9.0
// https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc?view=aspnetcore-9.0

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;
[Authorize]
[ApiController]
// [Route("[controller]")] // makes the endpoint of the controller to be /WeatherForecast, from the name of the controller i.e WeatherForecastController.
[Route("weather_forecast")] // defines what the name of the route is. Does not depend on the name of the controller
// see: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-9.0
// see: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-9.0
public class WeatherForecastController(/* ILogger logger /* *\/ The logger does not work, see program.cs for more info */) : ControllerBase // http://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase?view=aspnetcore-9.0
{
    private readonly string[] _summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    // [HttpGet("get", Name = "GetWeatherForecast_2")] // weather_forecast/get
    [HttpGet("", Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> GetFiveDayForecast()
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            _summaries[Random.Shared.Next(_summaries.Length)]
        ))
        .ToArray();
        // logger.LogDebug("Weather Warning"); // Does not Work
        // read more about ILogger https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.ilogger-1?view=net-9.0-pp
        return forecast;
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

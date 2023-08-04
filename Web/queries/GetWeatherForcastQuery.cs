using MediatR;
using LanguageExt;
using Domain.AuthMiddleware;
using Unit = LanguageExt.Unit;
namespace CQRSWithLangExtExample;

[AdminAccess][ApiUserAccess]
public record WeatherForecastQuery(DateTime Date, int TemperatureC, string? Summary) : IRequest<Either<string, IEnumerable<WeatherForecastReadModel>>>
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class WeatherForecastQueryHandler : IRequestHandler<WeatherForecastQuery,  Either<string, IEnumerable<WeatherForecastReadModel>>>
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public async Task<Either<string, IEnumerable<WeatherForecastReadModel>>> Handle(WeatherForecastQuery request, CancellationToken cancellationToken) =>
        await CurrentUserCanAccessWeatherDataRange()
            .BindAsync(async _ => await GetWeatherData());
    

    public async Task<Either<string, Unit>> CurrentUserCanAccessWeatherDataRange(){
        var userHasAccess = await Task.FromResult(true);
        if(!userHasAccess)
            return "Error current user has no access to weather data far in the past";

        return Unit.Default;
    }

    public async Task<Either<string, IEnumerable<WeatherForecastReadModel>>> GetWeatherData()
    {
        //Do some sql here to gather data using dapper map to return model
        return await Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecastReadModel
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray());
    }

}

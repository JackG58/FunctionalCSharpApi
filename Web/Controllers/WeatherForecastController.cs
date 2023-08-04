using Microsoft.AspNetCore.Mvc;
using MediatR;
using LanguageExt;

namespace CQRSWithLangExtExample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{

    private readonly ILogger<WeatherForecastController> _logger;

    private readonly IMediator _mediator;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Route("GetWeather")]
    public async Task<ActionResult<IEnumerable<WeatherForecastReadModel>>> Get()
    {
       var result = await _mediator.Send(new WeatherForecastQuery(DateTime.Now, 14, "Test"));
       return result.Match(HandleOk, HandleBadRequest);
    }


    private ActionResult HandleBadRequest<T>(T error) => BadRequest(error);
    private ActionResult HandleOk<T>(T ok) => Ok(ok);
}

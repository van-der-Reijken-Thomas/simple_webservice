using MCT.Functions.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MCT.Functions;

public class GetCoffeeNamesTrigger
{
    private readonly ILogger<GetCoffeeNamesTrigger> _logger;
    private readonly TransactionRepository _repository = new();

    public GetCoffeeNamesTrigger(ILogger<GetCoffeeNamesTrigger> logger)
    {
        _logger = logger;
    }

    [Function("GetCoffeeNames")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "coffees")] HttpRequest req)
    {
        try
        {
            var coffeeNames = await _repository.GetCoffeeNamesAsync();
            return new OkObjectResult(coffeeNames);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Could not retrieve coffee names.");

            return new ObjectResult(new { error = "Could not retrieve coffee names" })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}

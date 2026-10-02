using MCT.Functions.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MCT.Functions;

public class GetTransactionCountTrigger
{
    private readonly ILogger<GetTransactionCountTrigger> _logger;
    private readonly TransactionRepository _repository = new();

    public GetTransactionCountTrigger(ILogger<GetTransactionCountTrigger> logger)
    {
        _logger = logger;
    }

    [Function("GetTransactionCount")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "transactions/count")] HttpRequest req)
    {
        try
        {
            int count = await _repository.GetTransactionCountAsync();
            return new OkObjectResult(count);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Could not count transactions.");

            return new ObjectResult(new { error = "Could not count transactions" })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}

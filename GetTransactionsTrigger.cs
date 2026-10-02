using MCT.Functions.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MCT.Functions;

public class GetTransactionsTrigger
{
    private readonly ILogger<GetTransactionsTrigger> _logger;
    private readonly TransactionRepository _repository = new();

    public GetTransactionsTrigger(ILogger<GetTransactionsTrigger> logger)
    {
        _logger = logger;
    }

    [Function("GetTransactions")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "transactions")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Retrieving all coffee transactions.");

            var transactions = await _repository.GetTransactionsAsync();
            return new OkObjectResult(transactions);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Could not retrieve transactions.");

            return new ObjectResult(new { error = "Could not retrieve transactions" })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}

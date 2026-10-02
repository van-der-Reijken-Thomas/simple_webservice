using System.Text.Json;
using MCT.Functions.Models;
using MCT.Functions.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MCT.Functions;

public class CreateTransactionTrigger
{
    private readonly ILogger<CreateTransactionTrigger> _logger;
    private readonly TransactionRepository _repository = new();

    public CreateTransactionTrigger(ILogger<CreateTransactionTrigger> logger)
    {
        _logger = logger;
    }

    [Function("CreateTransaction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "transactions")] HttpRequest req)
    {
        try
        {
            var request = await JsonSerializer.DeserializeAsync<CreateCoffeeTransactionRequest>(
                req.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (request is null || request.DateTime == default ||
                string.IsNullOrWhiteSpace(request.CashType) ||
                string.IsNullOrWhiteSpace(request.Card) ||
                request.Money <= 0 ||
                string.IsNullOrWhiteSpace(request.CoffeeName))
            {
                return new BadRequestObjectResult(new
                {
                    error = "dateTime, cashType, card, positive money and coffeeName are required"
                });
            }

            var transaction = await _repository.CreateTransactionAsync(request);
            return new CreatedResult("/api/transactions", transaction);
        }
        catch (JsonException exception)
        {
            _logger.LogWarning(exception, "Invalid JSON received for a new transaction.");
            return new BadRequestObjectResult(new { error = "The request body must contain valid JSON" });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Could not create transaction.");

            return new ObjectResult(new { error = "Could not create transaction" })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}

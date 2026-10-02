using System.Text.Json;
using MCT.Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MCT.Functions;

public class CalculatorTrigger
{
    private readonly ILogger<CalculatorTrigger> _logger;

    public CalculatorTrigger(ILogger<CalculatorTrigger> logger)
    {
        _logger = logger;
    }

    [Function("CalculatorTrigger")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "calculator")] HttpRequest req)
    {
        CalculationRequest? request;

        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            request = JsonSerializer.Deserialize<CalculationRequest>(requestBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException)
        {
            return new BadRequestObjectResult("The request body must contain valid JSON with numeric a and b values.");
        }

        if (request is null || string.IsNullOrWhiteSpace(request.Operator))
        {
            return new BadRequestObjectResult("The request body must contain a, b and operator.");
        }

        string operation = request.Operator.Trim().ToLowerInvariant();
        double result;
        string operationSymbol;

        switch (operation)
        {
            case "+":
            case "plus":
                result = request.A + request.B;
                operationSymbol = "+";
                break;

            case "-":
            case "minus":
                result = request.A - request.B;
                operationSymbol = "-";
                break;

            case "*":
            case "multiply":
                result = request.A * request.B;
                operationSymbol = "*";
                break;

            case "/":
            case "divide":
            case "div":
                if (request.B == 0)
                {
                    return new BadRequestObjectResult("Division by zero is not allowed.");
                }

                result = request.A / request.B;
                operationSymbol = "/";
                break;

            default:
                return new BadRequestObjectResult(
                    "Unknown operator. Use +, -, *, /, plus, minus, multiply or divide.");
        }

        _logger.LogInformation(
            "Calculating {A} {Operator} {B}", request.A, operationSymbol, request.B);

        return new OkObjectResult(new CalculationResult
        {
            Result = result,
            Operation = operationSymbol
        });
    }
}

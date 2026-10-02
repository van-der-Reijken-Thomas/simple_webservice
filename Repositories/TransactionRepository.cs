using MCT.Functions.Models;
using Microsoft.Data.SqlClient;

namespace MCT.Functions.Repositories;

public class TransactionRepository
{
    private static string GetConnectionString() =>
        Environment.GetEnvironmentVariable("SqlConnectionString")
        ?? throw new InvalidOperationException("SqlConnectionString is not configured.");

    public async Task<List<CoffeeTransaction>> GetTransactionsAsync()
    {
        var transactions = new List<CoffeeTransaction>();

        await using var connection = new SqlConnection(GetConnectionString());
        await connection.OpenAsync();

        const string sql = "SELECT ID, DateTime, CashType, Card, Money, CoffeeName FROM Transactions";
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            transactions.Add(new CoffeeTransaction
            {
                ID = reader.GetGuid(0),
                DateTime = reader.GetDateTime(1),
                CashType = reader.GetString(2),
                Card = reader.GetString(3),
                Money = reader.GetDecimal(4),
                CoffeeName = reader.GetString(5)
            });
        }

        return transactions;
    }

    public async Task<List<string>> GetCoffeeNamesAsync()
    {
        var coffeeNames = new List<string>();

        await using var connection = new SqlConnection(GetConnectionString());
        await connection.OpenAsync();

        const string sql = "SELECT DISTINCT CoffeeName FROM Transactions ORDER BY CoffeeName";
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            coffeeNames.Add(reader.GetString(0));
        }

        return coffeeNames;
    }

    public async Task<CoffeeTransaction> CreateTransactionAsync(
        CreateCoffeeTransactionRequest request)
    {
        await using var connection = new SqlConnection(GetConnectionString());
        await connection.OpenAsync();

        const string sql = """
            INSERT INTO Transactions (DateTime, CashType, Card, Money, CoffeeName)
            OUTPUT INSERTED.ID, INSERTED.DateTime, INSERTED.CashType,
                   INSERTED.Card, INSERTED.Money, INSERTED.CoffeeName
            VALUES (@DateTime, @CashType, @Card, @Money, @CoffeeName)
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@DateTime", request.DateTime);
        command.Parameters.AddWithValue("@CashType", request.CashType);
        command.Parameters.AddWithValue("@Card", request.Card);
        command.Parameters.AddWithValue("@Money", request.Money);
        command.Parameters.AddWithValue("@CoffeeName", request.CoffeeName);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            throw new InvalidOperationException("The inserted transaction was not returned.");
        }

        return new CoffeeTransaction
        {
            ID = reader.GetGuid(0),
            DateTime = reader.GetDateTime(1),
            CashType = reader.GetString(2),
            Card = reader.GetString(3),
            Money = reader.GetDecimal(4),
            CoffeeName = reader.GetString(5)
        };
    }

    public async Task<int> GetTransactionCountAsync()
    {
        await using var connection = new SqlConnection(GetConnectionString());
        await connection.OpenAsync();

        const string sql = "SELECT COUNT(*) FROM Transactions";
        await using var command = new SqlCommand(sql, connection);

        object? result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }
}

using MCT.Functions.Models;
using Microsoft.Data.SqlClient;

namespace MCT.Functions.Repositories;

public class TransactionRepository
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("SqlConnectionString")
        ?? throw new InvalidOperationException("SqlConnectionString is not configured.");

    public async Task<List<CoffeeTransaction>> GetTransactionsAsync()
    {
        var transactions = new List<CoffeeTransaction>();

        await using var connection = new SqlConnection(_connectionString);
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
}
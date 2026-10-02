namespace MCT.Functions.Models;

public class CoffeeTransaction
{
    public Guid ID { get; set; }
    public DateTime DateTime { get; set; }
    public string CashType { get; set; } = string.Empty;
    public string Card { get; set; } = string.Empty;
    public decimal Money { get; set; }
    public string CoffeeName { get; set; } = string.Empty;
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Ticker
{

    public int Id { get; private set; }

    public string Symbol { get; private set; } = string.Empty;

    public decimal CurrentPrice { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    // EF only
    protected Ticker() { }

    public Ticker(string symbol, decimal currentPrice)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("O símbolo do ativo (ticker) não pode estar vazio.", nameof(symbol));

        if (currentPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(currentPrice), "O preço atual não pode ser negativo.");

        Symbol = symbol.Trim().ToUpperInvariant();
        CurrentPrice = currentPrice;
        var now = DateTime.Now;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(newPrice), "O preço não pode ser negativo.");

        CurrentPrice = newPrice;
        UpdatedAt = DateTime.Now;
    }
}
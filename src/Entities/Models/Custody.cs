using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Custody
{
    public int Id { get; private set; }

    public int CustomerId { get; private set; }

    public int TradingAccountId { get; private set; }

    public string Symbol { get; private set; } = string.Empty;

    public int Quantity { get; private set; }
    
    public decimal AveragePrice { get; private set; }

    // EF
    protected Custody() { }

    public Custody(int customerId, int tradingAccountId, string symbol, int quantity, decimal averagePrice)
    {
        if (customerId <= 0) 
            throw new ArgumentException("O ID do cliente é inválido.", nameof(customerId));
        if (tradingAccountId <= 0) 
            throw new ArgumentException("O ID da conta gráfica é inválido.", nameof(tradingAccountId));
        if (string.IsNullOrWhiteSpace(symbol)) 
            throw new ArgumentException("O símbolo do ativo (ticker) é obrigatório.", nameof(symbol));
        if (quantity < 0) 
            throw new ArgumentOutOfRangeException(nameof(quantity), "A quantidade não pode ser negativa.");
        if (averagePrice < 0) 
            throw new ArgumentOutOfRangeException(nameof(averagePrice), "O preço médio não pode ser negativo.");

        CustomerId = customerId;
        TradingAccountId = tradingAccountId;
        Symbol = symbol.Trim().ToUpperInvariant();
        Quantity = quantity;
        AveragePrice = averagePrice;
    }

    public void AddQuantity(int quantity, decimal unitPrice)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (unitPrice < 0) throw new ArgumentOutOfRangeException(nameof(unitPrice));

        var currentTotal = Quantity * AveragePrice;
        var newTotal = quantity * unitPrice;
        Quantity += quantity;
        AveragePrice = (currentTotal + newTotal) / Quantity;
    }

    public void RemoveQuantity(int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (quantity > Quantity) throw new InvalidOperationException("Insufficient quantity in custody.");

        Quantity -= quantity;
    }
}
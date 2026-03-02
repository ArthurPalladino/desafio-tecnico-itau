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
            throw new CustomException("ID_CLIENTE_INVALIDO");

        if (tradingAccountId <= 0) 
            throw new CustomException("ID_CONTA_INVALIDO");

        if (string.IsNullOrWhiteSpace(symbol)) 
            throw new CustomException("TICKER_OBRIGATORIO");

        if (quantity < 0) 
            throw new CustomException("QUANTIDADE_NEGATIVA");

        if (averagePrice < 0) 
            throw new CustomException("PRECO_NEGATIVO");

        CustomerId = customerId;
        TradingAccountId = tradingAccountId;
        Symbol = symbol.Trim().ToUpperInvariant();
        Quantity = quantity;
        AveragePrice = averagePrice;
    }

    public void AddQuantity(int quantity, decimal unitPrice)
    {
        if (quantity <= 0) 
            throw new CustomException("QUANTIDADE_NEGATIVA");

        if (unitPrice < 0) 
            throw new CustomException("PRECO_NEGATIVO");

        var currentTotal = Quantity * AveragePrice;
        var newTotal = quantity * unitPrice;
        Quantity += quantity;
        AveragePrice = (currentTotal + newTotal) / Quantity;
    }

    public void RemoveQuantity(int quantity)
    {
        if (quantity <= 0) 
            throw new CustomException("QUANTIDADE_NEGATIVA");

        if (quantity > Quantity) 
            throw new CustomException("CUSTODIA_INSUFICIENTE");

        Quantity -= quantity;
    }
}
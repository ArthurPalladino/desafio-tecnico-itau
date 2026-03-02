using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Rebalancing
{

    public int Id { get; private set; }

    public int CustomerId { get; private set; }

    public string SellTicker { get; private set; } = string.Empty;

    public string BuyTicker { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public DateTime RebalancingDate { get; private set; }

    public RebalancingType Type { get; private set; }

    // EF
    protected Rebalancing() { }

    public Rebalancing(int customerId, string sellTicker, string buyTicker, int quantity, RebalancingType type)
    {
        if (customerId <= 0) 
            throw new CustomException("ID_CLIENTE_INVALIDO");

        if (string.IsNullOrWhiteSpace(sellTicker)) 
            throw new CustomException("TICKER_OBRIGATORIO"); 

        if (string.IsNullOrWhiteSpace(buyTicker)) 
            throw new CustomException("TICKER_OBRIGATORIO");

        if (quantity <= 0) 
            throw new CustomException("QUANTIDADE_NEGATIVA");

        CustomerId = customerId;
        SellTicker = sellTicker.Trim().ToUpperInvariant();
        BuyTicker = buyTicker.Trim().ToUpperInvariant();
        Quantity = quantity;
        RebalancingDate = DateTime.Now;
        Type = type;
    }
}
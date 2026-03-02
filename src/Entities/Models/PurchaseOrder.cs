using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class PurchaseOrder
{
    public int Id { get; private set; }

    public int MasterAccountId { get; private set; }

    public string Symbol { get; private set; } = string.Empty;


    public int Quantity { get; private set; }


    public decimal UnitPrice { get; private set; }


    public DateTime ExecutionDate { get; private set; }

    public MarketType MarketType { get; private set; }

    // EF
    protected PurchaseOrder() { }

    public PurchaseOrder(int masterAccountId, string symbol, MarketType marketType, int quantity, decimal unitPrice)
    {
        if (masterAccountId <= 0) 
            throw new CustomException("CONTA_MASTER_INVALIDA");

        if (string.IsNullOrWhiteSpace(symbol)) 
            throw new CustomException("TICKER_OBRIGATORIO");

        if (quantity <= 0) 
            throw new CustomException("QUANTIDADE_NEGATIVA");

        if (unitPrice < 0) 
            throw new CustomException("PRECO_NEGATIVO");

        MasterAccountId = masterAccountId;
        Symbol = symbol.Trim().ToUpperInvariant();
        Quantity = quantity;
        UnitPrice = unitPrice;
        ExecutionDate = DateTime.Now;
        MarketType = marketType;
    }
}
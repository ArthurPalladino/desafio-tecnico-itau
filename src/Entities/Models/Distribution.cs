using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Distribution
{
    public int Id { get; private set; }

    public int PurchaseOrderId { get; private set; }

    public int ChildAccountId { get; private set; }

    public string Symbol { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public decimal ExecutionPrice { get; private set; }

    public DateTime DistributedAt { get; private set; }

    // EF
    protected Distribution() { }

    public Distribution(int purchaseOrderId, int childAccountId, string symbol, int quantity, decimal executionPrice)
    {
        if (purchaseOrderId <= 0) 
            throw new CustomException("ORDEM_COMPRA_INVALIDA");

        if (childAccountId <= 0) 
            throw new CustomException("CONTA_FILHOTE_INVALIDA");

        if (string.IsNullOrWhiteSpace(symbol)) 
            throw new CustomException("TICKER_OBRIGATORIO");

        if (quantity < 0) 
            throw new CustomException("QUANTIDADE_NEGATIVA");

        if (executionPrice < 0) 
            throw new CustomException("PRECO_NEGATIVO");

        PurchaseOrderId = purchaseOrderId;
        ChildAccountId = childAccountId;
        Symbol = symbol.Trim().ToUpperInvariant();
        Quantity = quantity;
        ExecutionPrice = executionPrice;
        DistributedAt = DateTime.Now;
    }
}
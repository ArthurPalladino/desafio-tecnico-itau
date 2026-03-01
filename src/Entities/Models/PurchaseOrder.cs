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
            throw new ArgumentException("A conta master informada é inválida.", nameof(masterAccountId));
        if (string.IsNullOrWhiteSpace(symbol)) 
            throw new ArgumentException("O símbolo do ativo (ticker) é obrigatório.", nameof(symbol));
        if (quantity <= 0) 
            throw new ArgumentOutOfRangeException(nameof(quantity), "A quantidade deve ser um valor positivo.");
        if (unitPrice < 0) 
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "O preço de execução não pode ser negativo.");

        MasterAccountId = masterAccountId;
        Symbol = symbol.Trim().ToUpperInvariant();
        Quantity = quantity;
        UnitPrice = unitPrice;
        ExecutionDate = DateTime.Now;
        MarketType = marketType;
    }
}
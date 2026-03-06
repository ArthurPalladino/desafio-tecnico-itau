public class Distribution
{
    public int Id { get; private set; }
    public int PurchaseOrderId { get; private set; }
    public int ChildAccountId { get; private set; }
    public string Symbol { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal ExecutionPrice { get; private set; }
    public DateTime DistributedAt { get; private set; }
    
    // Propriedade de navegação para o EF linkar os IDs automaticamente
    public virtual PurchaseOrder PurchaseOrder { get; private set; }

    // EF
    protected Distribution() { }

    // Construtor para uso durante o processamento (Master ainda sem ID)
    public Distribution(PurchaseOrder purchaseOrder, int childAccountId, string symbol, int quantity, decimal executionPrice, DateTime distributionDate)
    {
        if (purchaseOrder == null) 
            throw new CustomException("ORDEM_COMPRA_INVALIDA");

        ValidateFields(childAccountId, symbol, quantity, executionPrice);

        this.PurchaseOrder = purchaseOrder; 
        this.ChildAccountId = childAccountId;
        this.Symbol = symbol.Trim().ToUpperInvariant();
        this.Quantity = quantity;
        this.ExecutionPrice = executionPrice;
        this.DistributedAt = distributionDate;
    }

    public Distribution(int purchaseOrderId, int childAccountId, string symbol, int quantity, decimal executionPrice)
    {
        if (purchaseOrderId <= 0) 
            throw new CustomException("ORDEM_COMPRA_INVALIDA");

        ValidateFields(childAccountId, symbol, quantity, executionPrice);

        this.PurchaseOrderId = purchaseOrderId;
        this.ChildAccountId = childAccountId;
        this.Symbol = symbol.Trim().ToUpperInvariant();
        this.Quantity = quantity;
        this.ExecutionPrice = executionPrice;
        this.DistributedAt = DateTime.Now;
    }

    private void ValidateFields(int childAccountId, string symbol, int quantity, decimal executionPrice)
    {
        if (childAccountId <= 0) 
            throw new CustomException("CONTA_FILHOTE_INVALIDA");

        if (string.IsNullOrWhiteSpace(symbol)) 
            throw new CustomException("TICKER_OBRIGATORIO");

        if (quantity < 0) 
            throw new CustomException("QUANTIDADE_NEGATIVA");

        if (executionPrice < 0) 
            throw new CustomException("PRECO_NEGATIVO");
    }
}
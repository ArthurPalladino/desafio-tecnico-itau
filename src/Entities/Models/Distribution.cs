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
        throw new ArgumentException("A ordem de compra informada é inválida.", nameof(purchaseOrderId));
    if (childAccountId <= 0) 
        throw new ArgumentException("A conta filhote informada é inválida.", nameof(childAccountId));
    if (string.IsNullOrWhiteSpace(symbol)) 
        throw new ArgumentException("O símbolo do ativo (ticker) é inválido ou obrigatório.", nameof(symbol));
    if (quantity < 0) 
        throw new ArgumentOutOfRangeException(nameof(quantity), "A quantidade não pode ser negativa.");
    if (executionPrice < 0) 
        throw new ArgumentOutOfRangeException(nameof(executionPrice), "O preço de execução não pode ser negativo.");

        PurchaseOrderId = purchaseOrderId;
        ChildAccountId = childAccountId;
        Symbol = symbol.Trim().ToUpperInvariant();
        Quantity = quantity;
        ExecutionPrice = executionPrice;
        DistributedAt = DateTime.Now;
    }
}
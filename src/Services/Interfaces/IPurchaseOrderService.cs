public interface IPurchaseOrderService
{
    Task<List<PurchaseOrder>> CreatePurchaseOrder(int masterAccountId, string tickerBase, int qtyTotal, decimal unitPrice);
    Task CreateSellOrders(TradingAccount customerAccount, Custody customerCustody, decimal price);
}
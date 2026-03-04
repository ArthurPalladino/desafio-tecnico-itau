using Repositories.Interfaces;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    public PurchaseOrderService(IPurchaseOrderRepository purchaseOrderRepository)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
    }
    public async Task<List<PurchaseOrder>> CreatePurchaseOrder(int masterAccountId, string tickerBase, int qtyTotal, decimal unitPrice)
    {
        int standard = qtyTotal / 100 * 100;
        int fractional = qtyTotal % 100;

        var createdOrders = new List<PurchaseOrder>();
        if (fractional > 0)
        {
            var fracOrder = new PurchaseOrder(masterAccountId, tickerBase + "F", MarketType.Fractional, fractional, unitPrice);
            await _purchaseOrderRepository.AddAsync(fracOrder);
            createdOrders.Add(fracOrder);
        }

        if (standard > 0)
        {
            var standartOrder = new PurchaseOrder(masterAccountId, tickerBase, MarketType.StandardLot, standard, unitPrice);
            await _purchaseOrderRepository.AddAsync(standartOrder);
            createdOrders.Add(standartOrder);
        }
        return createdOrders;
    }

    public async Task CreateSellOrders(TradingAccount customerAccount, Custody customerCustody, decimal price)
    {
        int totalQuantity = customerCustody.Quantity;
        int standardLot = (totalQuantity / 100) * 100;
        int fractionalLot = totalQuantity % 100;

        if (standardLot > 0)
        {
            var standardOrder = new PurchaseOrder(customerAccount.Id, customerCustody.Symbol, MarketType.StandardLot, -standardLot, price);
            await _purchaseOrderRepository.AddAsync(standardOrder);
        }

        if (fractionalLot > 0)
        {
            var fractionalOrder = new PurchaseOrder(customerAccount.Id, customerCustody.Symbol + "F", MarketType.Fractional, -fractionalLot, price);
            await _purchaseOrderRepository.AddAsync(fractionalOrder);
        }
    }


}
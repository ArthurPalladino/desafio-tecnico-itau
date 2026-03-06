namespace Repositories.Interfaces
{
    public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
    {
        Task<decimal> GetTotalSalesValueInMonthAsync(long accountId, int year, int month);
        Task<PurchaseOrder?> GetLastOrderAsync(string symbol);
        Task<IEnumerable<PurchaseOrder>> GetAllByBuyerId (int buyerId);

    }
}
namespace Repositories.Interfaces
{
    public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
    {
        Task<decimal> GetTotalSalesValueInMonthAsync(long accountId, int year, int month);
        public Task<PurchaseOrder?> GetLastOrderAsync(string symbol);

    }
}
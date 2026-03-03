namespace Repositories.Interfaces
{
    public interface ITaxEventRepository : IRepository<TaxEvent>
    {
        Task<decimal> GetTotalBaseValueInMonthAsync(long customerId, TaxType type, int year, int month);
        Task<decimal> GetTotalIRValueInMonthAsync(long customerId, TaxType type, int year, int month);
    }
}
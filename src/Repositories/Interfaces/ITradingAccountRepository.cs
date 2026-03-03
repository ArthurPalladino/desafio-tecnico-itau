namespace Repositories.Interfaces
{
    public interface ITradingAccountRepository : IRepository<TradingAccount>
    {
        Task<TradingAccount?> GetByCustomerIdAsync(int id);

        Task<TradingAccount?> GetMasterAccount();
    }
}
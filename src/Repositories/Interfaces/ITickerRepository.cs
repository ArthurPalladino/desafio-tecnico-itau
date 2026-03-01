namespace Repositories.Interfaces
{
    public interface ITickerRepository : IRepository<Ticker>
    {
        Task<Ticker?> GetBySymbolAsync(string symbol);
    }
}
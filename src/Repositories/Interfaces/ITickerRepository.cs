namespace Repositories.Interfaces
{
    public interface ITickerRepository : IRepository<Ticker>
    {
        Task<Ticker?> GetBySymbolAsync(string symbol);
        Task<Dictionary<string ,Ticker>> GetTickersByDateDictAsync(DateTime date);
        
    }
}
namespace Repositories.Interfaces
{
    public interface ITickerRepository : IRepository<Ticker>
    {
        Task<Ticker?> GetBySymbolAsync(string symbol, DateTime referenceDate);
        Task<Ticker?> GetLatestByTickerAsync(string symbol);
        Task<Dictionary<string, Ticker>> GetTickersDictBySymbol(List<string> symbols);
        Task<Dictionary<string, Ticker>> GetTickersByDateDictAsync(DateTime date);
        
    }
}
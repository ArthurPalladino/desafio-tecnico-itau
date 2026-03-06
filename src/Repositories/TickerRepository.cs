using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;

public class TickerRepository(ItauTopFiveDbContext dbContext) : Repository<Ticker>(dbContext), ITickerRepository
{
    public async Task<List<String>> GetUniqueSymbols()
    {
            return await _dbSet
            .Select(t => t.Symbol)
            .Distinct()
            .ToListAsync();
    }

    public async Task<Ticker?> GetBySymbolAsync(string symbol, DateTime referenceDate)
    {
        return await _dbSet
        .Where(c => c.Symbol.ToLower() == symbol.ToLower()
                    && c.PriceDate <= referenceDate)
        .OrderByDescending(c => c.PriceDate)
        .FirstOrDefaultAsync();    
    }

    public async Task<Ticker?> GetLatestByTickerAsync(string symbol)
    {
            return await _dbSet
            .Where(c => c.Symbol.ToLower() == symbol.ToLower())
            .OrderByDescending(c => c.PriceDate)
            .FirstOrDefaultAsync();   
    }

    public async Task<Dictionary<string, Ticker>> GetTickersByDateDictAsync(DateTime date)
    {
    return await _context.Tickers
        .Where(t => t.PriceDate == date)
        .ToDictionaryAsync(t => t.Symbol, t => t); 
    }

    public async Task<Dictionary<string, Ticker>> GetTickersDictByLastDate()
    {
        var lastDate = await _dbSet.Select(t => (DateTime?)t.PriceDate).MaxAsync();

        if (lastDate == null) return null;

        var tickers = await _dbSet
            .Where(t => t.PriceDate == lastDate)
            .ToDictionaryAsync(t => t.Symbol, t => t);

        return tickers;
    }

    public async Task<Dictionary<string, Ticker>> GetTickersDictBySymbol(List<string> symbols)
    {
        var allTickers = await _context.Tickers
            .Where(t => symbols.Contains(t.Symbol))
            .ToListAsync();

        return allTickers
            .GroupBy(t => t.Symbol)
            .Select(g => g.OrderByDescending(t => t.PriceDate).First())
            .ToDictionary(t => t.Symbol, t => t);
    }
    public async Task<IEnumerable<DateTime>> GetDistinctDatesAsync()
    {
        return await _context.Tickers
            .Select(t => t.PriceDate)
            .Distinct()
            .ToListAsync();
    }

    public async Task<Ticker> GetPriceAtDate(string symbol, DateTime date)
    {
        var ticker = await _context.Tickers
            .Where(t => t.Symbol == symbol && t.PriceDate.Date == date.Date)
            .OrderByDescending(t => t.PriceDate) 
            .FirstOrDefaultAsync();

        if (ticker == null)
        {
            ticker = await _context.Tickers
                .Where(t => t.Symbol == symbol && t.PriceDate.Date < date.Date)
                .OrderByDescending(t => t.PriceDate)
                .FirstOrDefaultAsync();
        }

        return ticker;
    }
}
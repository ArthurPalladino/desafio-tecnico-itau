using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;

public class TickerRepository(ItauTopFiveDbContext dbContext) : Repository<Ticker>(dbContext), ITickerRepository
{
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
    
}
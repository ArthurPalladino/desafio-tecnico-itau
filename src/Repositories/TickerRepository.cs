using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;

public class TickerRepository(ItauTopFiveDbContext dbContext) : Repository<Ticker>(dbContext), ITickerRepository
{
    public async Task<Ticker?> GetBySymbolAsync(string symbol)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Symbol.ToLower() == symbol.ToLower());    
    }

    public async Task<Dictionary<string, Ticker>> GetTickersByDateDictAsync(DateTime date)
    {
        return await _context.Tickers
            .Where(t => t.PriceDate == date)
            .ToDictionaryAsync(t => t.Symbol, t => t); 
    }
    
}
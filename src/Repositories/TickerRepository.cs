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
}
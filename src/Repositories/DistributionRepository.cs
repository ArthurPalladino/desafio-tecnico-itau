using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;

public class DistributionRepository(ItauTopFiveDbContext dbContext) : Repository<Distribution>(dbContext), IDistributionRepository
{
    public async Task<Distribution?> GetLatestDistributionByTickerAsync(string symbol)
    {
        return await _dbSet
            .Where(d => d.Symbol == symbol.ToUpper().Trim())
            .OrderByDescending(d => d.DistributedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Distribution>> GetAllDistributionsToChildAccount(int childAccountId)
    {
        return await _dbSet
        .Where(d => d.ChildAccountId == childAccountId)
        .ToListAsync();
    }
}
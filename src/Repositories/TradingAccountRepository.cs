using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;

public class TradingAccountRepository(ItauTopFiveDbContext dbContext) : Repository<TradingAccount>(dbContext), ITradingAccountRepository
{
    public async Task<TradingAccount?> GetByCustomerIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.CustomerId == id);    
    }

    public async Task<TradingAccount?> GetMasterAccount()
    {
        return await _dbSet
        .Include(ta => ta.Custodies)
        .FirstOrDefaultAsync(ta => ta.Id == 1);
    }
}
using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;


public class CustodyRepository(ItauTopFiveDbContext dbContext) : Repository<Custody>(dbContext), ICustodyRepository
{
    public async Task<Custody?> GetByAccountAndTickerAsync(int accountId, string ticker)
    {
        return await _context.Custodies
            .FirstOrDefaultAsync(c => c.CustomerId == accountId 
                                   && c.Symbol.ToUpper() == ticker.ToUpper());
    }
}
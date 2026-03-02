using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
public class ContributionHistoryRepository(ItauTopFiveDbContext dbContext) 
    : Repository<ContributionHistory>(dbContext), IContributionHistoryRepository
{
    public async Task<IEnumerable<ContributionHistory>> GetByCustomerIdAsync(int customerId)
    {
        return await _dbSet.Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.AlterationDate)
            .ToListAsync();
    }
}
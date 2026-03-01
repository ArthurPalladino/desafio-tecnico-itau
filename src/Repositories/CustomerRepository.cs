using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;

public class CustomerRepository(ItauTopFiveDbContext dbContext) : Repository<Customer>(dbContext), ICustomerRepository
{
    public async Task<IEnumerable<Customer>> GetActiveCustomers()
    {
        return await _dbSet.Where(c => c.IsActive).ToListAsync();    
    }

    public async Task<Customer?> GetCustomerWithPortfolioAsync(int customerId)
{
    return await _dbSet
        .Include(c => c.TradingAccount)         
        .ThenInclude(a => a.Custodies)      
        .FirstOrDefaultAsync(c => c.Id == customerId);
}
}
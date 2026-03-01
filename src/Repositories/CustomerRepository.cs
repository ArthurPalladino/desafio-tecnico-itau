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
}
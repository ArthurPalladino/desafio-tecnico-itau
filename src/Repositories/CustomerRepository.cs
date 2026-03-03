using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;

public class CustomerRepository(ItauTopFiveDbContext dbContext) : Repository<Customer>(dbContext), ICustomerRepository
{
    public async Task<IEnumerable<Customer>> GetActiveCustomers()
    {
        return await _context.Customers
            .Include(c => c.TradingAccount)
                .ThenInclude(ta => ta.Custodies)
            .Where(c => c.IsActive &&
                        c.TradingAccount.Type != AccountType.Master)
            .ToListAsync();
    }

    public async Task<Customer?> GetCustomerByCpf(string cpf)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Cpf == cpf);   
    }

    public async Task<Customer?> GetCustomerWithPortfolioAsync(int customerId)
{
    return await _dbSet
        .Include(c => c.TradingAccount)         
        .ThenInclude(a => a.Custodies)      
        .FirstOrDefaultAsync(c => c.Id == customerId);
}
}
using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;

public class TaxEventRepository(ItauTopFiveDbContext dbContext) : Repository<TaxEvent>(dbContext), ITaxEventRepository
{
    public async Task<decimal> GetTotalBaseValueInMonthAsync(long customerId, TaxType type, int year, int month)
    {
        return await _dbSet.Where(e => e.CustomerId == customerId &&
                        e.Type == type &&
                        e.EventDate.Year == year &&
                        e.EventDate.Month == month)
            .SumAsync(e => e.BaseAmount);
    }

    public async Task<decimal> GetTotalIRValueInMonthAsync(long customerId, TaxType type, int year, int month)
    {
        return await _dbSet
            .Where(e => e.CustomerId == customerId &&
                        e.Type == type &&
                        e.EventDate.Year == year &&
                        e.EventDate.Month == month)
            .SumAsync(e => e.TaxAmount);
    }
}
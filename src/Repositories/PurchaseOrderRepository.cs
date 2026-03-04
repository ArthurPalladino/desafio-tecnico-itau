using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;

public class PurchaseOrderRepository(ItauTopFiveDbContext dbContext) : Repository<PurchaseOrder>(dbContext), IPurchaseOrderRepository
{
    public async Task<PurchaseOrder?> GetLastOrderAsync(string symbol)
    {
        return await _context.PurchaseOrders
            .Where(o => o.Symbol == symbol)
            .OrderByDescending(o => o.ExecutionDate) 
            .FirstOrDefaultAsync();
    }

    public async Task<decimal> GetTotalSalesValueInMonthAsync(long accountId, int year, int month)
    {
        return await _dbSet.Where(o => o.MasterAccountId == accountId &&
                        o.Quantity < 0 &&
                        o.ExecutionDate.Year == year &&
                        o.ExecutionDate.Month == month)
            .SumAsync(o => (decimal)Math.Abs(o.Quantity) * o.UnitPrice);
    }
}
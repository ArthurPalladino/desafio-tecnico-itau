using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;

public class RecommendationBasketRepository(ItauTopFiveDbContext dbContext) : Repository<RecommendationBasket>(dbContext), IRecommendationBasketRepository
{
    public async Task<RecommendationBasket?> GetActiveBasketWithItensAsync()
    {
        return await _dbSet
        .Include(b => b.Itens)
        .FirstOrDefaultAsync(b => b.IsActive);
    }

    public async Task<List<RecommendationBasket>> GetHistoryAsync()
    {
        return await _context.RecommendationBaskets
        .Include(b => b.Itens)
        .OrderByDescending(b => b.CreatedAt)
        .ToListAsync();
    }
}
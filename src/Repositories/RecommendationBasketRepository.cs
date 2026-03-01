using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;

public class RecommendationBasketRepository(ItauTopFiveDbContext dbContext) : Repository<RecommendationBasket>(dbContext), IRecommendationBasketRepository
{
    public async Task<RecommendationBasket?> GetActiveBasketWithItemsAsync()
    {
        return await _dbSet
        .Include(b => b.Items)
        .FirstOrDefaultAsync(b => b.IsActive);
    }

}
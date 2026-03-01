using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;

public class RecommendationBasketRepository(ItauTopFiveDbContext dbContext) : Repository<RecommendationBasket>(dbContext), IRecommendationBasketRepository
{
    public async Task<RecommendationBasket?> GetActiveBasketAsync()
    {
        return await _dbSet.Where(c => c.IsActive).FirstOrDefaultAsync(); 
    }

}
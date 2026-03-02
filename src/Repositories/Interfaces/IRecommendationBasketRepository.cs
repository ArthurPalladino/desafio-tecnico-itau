namespace Repositories.Interfaces
{
    public interface IRecommendationBasketRepository : IRepository<RecommendationBasket>
    {
        Task<RecommendationBasket?> GetActiveBasketWithItensAsync();
        Task<List<RecommendationBasket>> GetHistoryAsync();
    }
}
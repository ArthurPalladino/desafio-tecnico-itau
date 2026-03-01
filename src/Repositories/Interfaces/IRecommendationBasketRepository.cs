namespace Repositories.Interfaces
{
    public interface IRecommendationBasketRepository : IRepository<RecommendationBasket>
    {
        Task<RecommendationBasket?> GetActiveBasketWithItemsAsync();
    }
}
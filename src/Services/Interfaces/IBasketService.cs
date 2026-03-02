public interface IBasketService
{
    Task<int> CreateAsync(CreateBasketRequest request);
    Task<RecommendationBasket?> GetActiveBasketAsync();
}


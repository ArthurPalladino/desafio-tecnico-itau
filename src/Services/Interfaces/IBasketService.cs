public interface IRecommendationBasketService
{
    Task<CreateBasketResponse> CreateAsync(CreateBasketRequest request);
    Task<BasketAtualResponse> GetActiveBasketAsync();

    Task<BasketHistoryResponse> GetHistoryAsync();
}


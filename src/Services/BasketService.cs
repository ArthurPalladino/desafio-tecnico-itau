using Repositories.Interfaces;

public class BasketService : IBasketService
{
    private readonly IRecommendationBasketRepository _recommendationBasketRepository; 

    public BasketService(IRecommendationBasketRepository recommendationBasketRepository)
    {
        _recommendationBasketRepository = recommendationBasketRepository;
    }

    public async Task<int> CreateAsync(CreateBasketRequest request)
    {
        var basketItems = request.Items
            .Select(dto => new BasketItem(dto.Symbol, dto.Percentage))
            .ToList();

        var newBasket = new RecommendationBasket(request.Name, basketItems);

        var activeBasket = await GetActiveBasketAsync();
        if (activeBasket != null)
        {
            activeBasket.Deactivate();
        }

        await _recommendationBasketRepository.AddAsync(newBasket);
        await _recommendationBasketRepository.SaveChangesAsync();

        return newBasket.Id;
    }

    public async Task<RecommendationBasket?> GetActiveBasketAsync()
    {
        return await _recommendationBasketRepository.GetActiveBasketWithItemsAsync();
    }
}
using Repositories.Interfaces;

public class RecommendationBasketService : IRecommendationBasketService
{
    private readonly IRecommendationBasketRepository _recommendationBasketRepository;
    private readonly IRebalancingEngineService _rebalancingEngineService;
    private readonly ITickerRepository _tickerRepository;

    public RecommendationBasketService(IRecommendationBasketRepository recommendationBasketRepository,
        IRebalancingEngineService rebalancingEngineService,
        ITickerRepository tickerRepository)
    {
        _tickerRepository = tickerRepository;
        _recommendationBasketRepository = recommendationBasketRepository;
        _rebalancingEngineService = rebalancingEngineService;
    }

    public async Task<CreateBasketResponse> CreateAsync(CreateBasketRequest request)
    {

        var basketItens = request.itens
            .Select(dto => new BasketItem(dto.ticker, dto.percentual))
            .ToList();

        

        var existingSymbols = new HashSet<string>(await _tickerRepository.GetUniqueSymbols());
        var unknownSymbols = basketItens
        .Select(i => i.Symbol)
        .Where(symbol => !existingSymbols.Contains(symbol))
        .ToList();

        if (unknownSymbols.Any())
        {
            var missing = string.Join(", ", unknownSymbols);
            throw new CustomException("TICKER_NAO_ENCONTRADO", missing); 
        }

        var newBasket = new RecommendationBasket(request.nome, basketItens);

        var activeBasket = await _recommendationBasketRepository.GetActiveBasketWithItensAsync();
        CestaAnteriorInfo anteriorInfo = null;

        if (activeBasket != null)
        {
            activeBasket.Deactivate();
            anteriorInfo = new CestaAnteriorInfo 
            { 
                cestaId = activeBasket.Id, 
                nome = activeBasket.Name, 
                dataDesativacao = DateTime.UtcNow 
            };
        }

        await _recommendationBasketRepository.AddAsync(newBasket);
        await _recommendationBasketRepository.SaveChangesAsync();
        bool rebalancingTrigger = await _rebalancingEngineService.ExecuteAsync(RebalancingType.RecommendationChange);


        return new CreateBasketResponse
        {
            cestaId = newBasket.Id,
            nome = newBasket.Name,
            ativa = true,
            dataCriacao = DateTime.UtcNow,
            itens = newBasket.Itens.Select(i => new CreateBasketItemResponse 
            { 
                ticker = i.Symbol, 
                percentual = i.Percentage 
            }).ToList(),
            rebalanceamentoDisparado = rebalancingTrigger,
            mensagem = activeBasket == null ? "Primeira cesta cadastrada com sucesso." : "Cesta atualizada com sucesso.",
            cestaAnteriorDesativada = anteriorInfo
        };
    }

    public async Task<BasketAtualResponse> GetActiveBasketAsync()
    {
        var basket = await _recommendationBasketRepository.GetActiveBasketWithItensAsync();
        
        if (basket == null)
            throw new CustomException("CESTA_NAO_ENCONTRADA");

        return new BasketAtualResponse
        {
            cestaId = basket.Id,
            nome = basket.Name,
            ativa = true,
            dataCriacao = basket.CreatedAt,
            itens = basket.Itens.Select(i => new BasketItemAtualResponse 
            { 
                ticker = i.Symbol, 
                percentual = i.Percentage,
                cotacaoAtual = _tickerRepository.GetLatestByTickerAsync(i.Symbol).Result.CurrentPrice
            }).ToList()
        };
    }

    public async Task<BasketHistoryResponse> GetHistoryAsync()
    {
        var baskets = await _recommendationBasketRepository.GetHistoryAsync();

        return new BasketHistoryResponse
        {
            cestas = baskets.Select(b => new BasketSummaryResponse
            {
                cestaId = b.Id,
                nome = b.Name,
                ativa = b.IsActive,
                dataCriacao = b.CreatedAt,
                dataDesativacao = b.DeactivatedAt, 
                
                itens = b.Itens.Select(i => new CreateBasketItemResponse
                {
                    ticker = i.Symbol,
                    percentual = i.Percentage
                }).ToList()
            }).ToList()
        };
    }
}
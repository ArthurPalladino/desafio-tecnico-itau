using Xunit;
using Moq;
using FluentAssertions;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Services.Tests
{
    public class RecommendationBasketServiceTests
    {
        private readonly Mock<IRecommendationBasketRepository> _basketRepoMock;
        private readonly Mock<IRebalancingEngineService> _rebalancingEngineServiceMock;
        private readonly Mock<ITickerRepository> _tickerRepoMock;
        private readonly RecommendationBasketService _service;

        public RecommendationBasketServiceTests()
        {
            _basketRepoMock = new Mock<IRecommendationBasketRepository>();
            _rebalancingEngineServiceMock = new Mock<IRebalancingEngineService>();
            _tickerRepoMock = new Mock<ITickerRepository>();

            _service = new RecommendationBasketService(
                _basketRepoMock.Object,
                _rebalancingEngineServiceMock.Object,
                _tickerRepoMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateBasket_WhenDataIsNominal()
        {
            var request = new CreateBasketRequest
            {
                nome = "Basket1",
                itens = new List<BasketItemRequest>
                {
                    new BasketItemRequest { ticker = "PETR4", percentual = 20 },
                    new BasketItemRequest { ticker = "VALE3", percentual = 20 },
                    new BasketItemRequest { ticker = "ITUB4", percentual = 20 },
                    new BasketItemRequest { ticker = "BBDC4", percentual = 20 },
                    new BasketItemRequest { ticker = "ABEV3", percentual = 20 }
                }
            };

            _tickerRepoMock.Setup(x => x.GetUniqueSymbols()).ReturnsAsync(new List<string> { "PETR4", "VALE3", "ITUB4", "BBDC4", "ABEV3" });
            _basketRepoMock.Setup(x => x.GetActiveBasketWithItensAsync()).ReturnsAsync((RecommendationBasket?)null);
            _basketRepoMock.Setup(x => x.AddAsync(It.IsAny<RecommendationBasket>())).Returns(Task.CompletedTask);
            _basketRepoMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _rebalancingEngineServiceMock.Setup(x => x.ExecuteAsync(RebalancingType.RecommendationChange)).ReturnsAsync(true);

            var result = await _service.CreateAsync(request);

            result.Should().NotBeNull();
            result.nome.Should().Be("Basket1");
            result.ativa.Should().BeTrue();
            result.itens.Should().HaveCount(5);
            _basketRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _rebalancingEngineServiceMock.Verify(x => x.ExecuteAsync(RebalancingType.RecommendationChange), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenTickerIsDuplicate()
        {
            var request = new CreateBasketRequest
            {
                nome = "Basket1",
                itens = new List<BasketItemRequest>
                {
                    new BasketItemRequest { ticker = "PETR4", percentual = 20 },
                    new BasketItemRequest { ticker = "PETR4", percentual = 20 },
                    new BasketItemRequest { ticker = "VALE3", percentual = 20 },
                    new BasketItemRequest { ticker = "ITUB4", percentual = 20 },
                    new BasketItemRequest { ticker = "BBDC4", percentual = 20 }
                }
            };

            Func<Task> act = async () => await _service.CreateAsync(request);

            await act.Should().ThrowAsync<CustomException>().WithMessage("A cesta não pode conter tickers duplicados.");
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenTickerNotFound()
        {
            var request = new CreateBasketRequest
            {
                nome = "Basket1",
                itens = new List<BasketItemRequest>
                {
                    new BasketItemRequest { ticker = "PETR4", percentual = 20 },
                    new BasketItemRequest { ticker = "INVALIDTICKER", percentual = 20 },
                    new BasketItemRequest { ticker = "ITUB4", percentual = 20 },
                    new BasketItemRequest { ticker = "BBDC4", percentual = 20 },
                    new BasketItemRequest { ticker = "ABEV3", percentual = 20 }
                }
            };

            _tickerRepoMock.Setup(x => x.GetUniqueSymbols()).ReturnsAsync(new List<string> { "PETR4", "ITUB4", "BBDC4", "ABEV3" });

            Func<Task> act = async () => await _service.CreateAsync(request);

            await act.Should().ThrowAsync<CustomException>();
        }

        [Fact]
        public async Task GetActiveBasketAsync_ShouldReturnBasket_WhenExists()
        {
            var itens = new List<BasketItem>
    {
        new BasketItem("PETR4", 20),
        new BasketItem("VALE3", 20),
        new BasketItem("ITUB4", 20),
        new BasketItem("BBDC4", 20),
        new BasketItem("ABEV3", 20)
    };
            var basket = new RecommendationBasket("TestBasket", itens);

            _basketRepoMock.Setup(x => x.GetActiveBasketWithItensAsync()).ReturnsAsync(basket);

            var tickersDict = itens.ToDictionary(
                i => i.Symbol,
                i => new Ticker(i.Symbol, 10, DateTime.Now)
            );

            _tickerRepoMock.Setup(x => x.GetTickersDictBySymbol(It.IsAny<List<string>>()))
                           .ReturnsAsync(tickersDict);

            var result = await _service.GetActiveBasketAsync();

            result.Should().NotBeNull();
            result.ativa.Should().BeTrue();
        }

        [Fact]
        public async Task GetActiveBasketAsync_ShouldThrowException_WhenBasketNotFound()
        {
            _basketRepoMock.Setup(x => x.GetActiveBasketWithItensAsync()).ReturnsAsync((RecommendationBasket?)null);

            Func<Task> act = async () => await _service.GetActiveBasketAsync();

            await act.Should().ThrowAsync<CustomException>().WithMessage("Nenhuma cesta ativa encontrada.");
        }

        [Fact]
        public async Task GetHistoryAsync_ShouldReturnHistory()
        {
            var itens = new List<BasketItem>
            {
                new BasketItem("PETR4", 20),
                new BasketItem("VALE3", 20),
                new BasketItem("ITUB4", 20),
                new BasketItem("BBDC4", 20),
                new BasketItem("ABEV3", 20)
            };
            var baskets = new List<RecommendationBasket> { new RecommendationBasket("Basket1", itens) };

            _basketRepoMock.Setup(x => x.GetHistoryAsync()).ReturnsAsync(baskets);

            var result = await _service.GetHistoryAsync();

            result.Should().NotBeNull();
            result.cestas.Should().HaveCount(1);
        }
    }
}

  

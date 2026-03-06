using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Controllers.Tests
{
    public class BasketControllerTests
    {
        private readonly Mock<IRecommendationBasketService> _basketServiceMock;
        private readonly BasketController _controller;

        public BasketControllerTests()
        {
            _basketServiceMock = new Mock<IRecommendationBasketService>();
            _controller = new BasketController(_basketServiceMock.Object);
        }

        [Fact]
        public async Task Create_ShouldReturnCreated_WhenDataIsNominal()
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

            var response = new CreateBasketResponse
            {
                cestaId = 1,
                nome = "Basket1",
                ativa = true,
                dataCriacao = System.DateTime.UtcNow,
                itens = new List<CreateBasketItemResponse>(),
                ativosAdicionados = new List<string>(),
                ativosRemovidos = new List<string>(),
                rebalanceamentoDisparado = true,
                mensagem = "Cesta criada",
                cestaAnteriorDesativada = null
            };

            _basketServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateBasketRequest>())).ReturnsAsync(response);

            var result = await _controller.Create(request);

            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenTickerDuplicate()
        {
            var request = new CreateBasketRequest
            {
                nome = "Basket1",
                itens = new List<BasketItemRequest>
                {
                    new BasketItemRequest { ticker = "PETR4", percentual = 20 },
                    new BasketItemRequest { ticker = "PETR4", percentual = 20 }
                }
            };

            _basketServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateBasketRequest>())).ThrowsAsync(new CustomException("A cesta não pode conter tickers duplicados."));

            Func<Task> act = async () => await _controller.Create(request);

            await act.Should().ThrowAsync<CustomException>();
        }

        [Fact]
        public async Task GetActive_ShouldReturnOk_WhenBasketExists()
        {
            var response = new BasketAtualResponse
            {
                cestaId = 1,
                nome = "Basket1",
                ativa = true,
                dataCriacao = System.DateTime.UtcNow,
                itens = new List<BasketItemAtualResponse>()
            };

            _basketServiceMock.Setup(x => x.GetActiveBasketAsync()).ReturnsAsync(response);

            var result = await _controller.GetActive();

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetActive_ShouldThrow_WhenBasketNotFound()
        {
            _basketServiceMock.Setup(x => x.GetActiveBasketAsync()).ThrowsAsync(new CustomException("Nenhuma cesta ativa encontrada."));

            Func<Task> act = async () => await _controller.GetActive();

            await act.Should().ThrowAsync<CustomException>();
        }

        [Fact]
        public async Task GetHistory_ShouldReturnOk()
        {
            var response = new BasketHistoryResponse
            {
                cestas = new List<BasketSummaryResponse>()
            };

            _basketServiceMock.Setup(x => x.GetHistoryAsync()).ReturnsAsync(response);

            var result = await _controller.GetHistory();

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
        }
    }
}

using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Tests.Controllers.Tests
{
    public class CustomersControllerTests
    {
        private readonly Mock<ICustomerService> _customerServiceMock;
        private readonly CustomersController _controller;

        public CustomersControllerTests()
        {
            _customerServiceMock = new Mock<ICustomerService>();
            _controller = new CustomersController(_customerServiceMock.Object);
        }

        [Fact]
        public async Task Create_ShouldReturnCreated_WhenDataIsNominal()
        {
            var request = new CreateCustomerRequest
            (
                 "Test Customer",
                 "12345678909",
                "test@example.com",
                 300
            );

            var response = new CustomerResponse(
                1,
                "Test Customer",
                "12345678909",
                "test@example.com",
                300,
                true,
                DateTime.UtcNow,
                new GraphicAccountResponse(1, "FLH-000001", "FILHOTE", DateTime.UtcNow)
            );

            _customerServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateCustomerRequest>())).ReturnsAsync(response);

            var result = await _controller.Create(request);

            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenValueTooLow()
        {
            var request = new CreateCustomerRequest
            (
               "Test",
               "12345678909",
               "test@example.com",
               50
            );

            _customerServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateCustomerRequest>())).ThrowsAsync(new CustomException("O valor mensal minimo e de R$ 100,00."));

            Func<Task> act = async () => await _controller.Create(request);

            await act.Should().ThrowAsync<CustomException>();
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenCpfDuplicate()
        {
            var request = new CreateCustomerRequest
            (
                  "Test",
                  "12345678909",
                  "test@example.com",
                  300
            );

            _customerServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateCustomerRequest>())).ThrowsAsync(new CustomException("CPF ja cadastrado no sistema."));

            Func<Task> act = async () => await _controller.Create(request);

            await act.Should().ThrowAsync<CustomException>();
        }

        [Fact]
        public async Task Leave_ShouldReturnOk_WhenCustomerExists()
        {
            var response = new SubscriptionChangeResponse(
                1,
                "Test Customer",
                false,
                DateTime.UtcNow,
                "Adesao encerrada."
            );

            _customerServiceMock.Setup(x => x.LeaveInvestmentProductAsync(It.IsAny<int>())).ReturnsAsync(response);

            var result = await _controller.Leave(1);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Leave_ShouldThrow_WhenCustomerNotFound()
        {
            _customerServiceMock.Setup(x => x.LeaveInvestmentProductAsync(It.IsAny<int>())).ThrowsAsync(new CustomException("Cliente nao encontrado."));

            Func<Task> act = async () => await _controller.Leave(999);

            await act.Should().ThrowAsync<CustomException>();
        }

        [Fact]
        public async Task Leave_ShouldThrow_WhenAlreadyInactive()
        {
            _customerServiceMock.Setup(x => x.LeaveInvestmentProductAsync(It.IsAny<int>())).ThrowsAsync(new CustomException("Cliente ja havia saido do produto."));

            Func<Task> act = async () => await _controller.Leave(1);

            await act.Should().ThrowAsync<CustomException>();
        }

        [Fact]
        public async Task UpdateAmount_ShouldReturnOk_WhenValid()
        {
            var response = new UpdateAmountResponse(
                1,
                300,
                500,
                DateTime.UtcNow,
                "Valor mensal atualizado."
            );

            _customerServiceMock.Setup(x => x.UpdateMonthlyAmountAsync(It.IsAny<int>(), It.IsAny<decimal>())).ReturnsAsync(response);

            var result = await _controller.UpdateAmount(1, 500);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task UpdateAmount_ShouldThrow_WhenValueTooLow()
        {
            _customerServiceMock.Setup(x => x.UpdateMonthlyAmountAsync(It.IsAny<int>(), It.IsAny<decimal>())).ThrowsAsync(new CustomException("O valor mensal minimo e de R$ 100,00."));

            Func<Task> act = async () => await _controller.UpdateAmount(1, 50);

            await act.Should().ThrowAsync<CustomException>();
        }

        [Fact]
        public async Task UpdateAmount_ShouldThrow_WhenCustomerNotFound()
        {
            _customerServiceMock.Setup(x => x.UpdateMonthlyAmountAsync(It.IsAny<int>(), It.IsAny<decimal>())).ThrowsAsync(new CustomException("Cliente nao encontrado."));

            Func<Task> act = async () => await _controller.UpdateAmount(999, 500);

            await act.Should().ThrowAsync<CustomException>();
        }

        [Fact]
        public async Task UpdateAmount_ShouldThrow_WhenValueSame()
        {
            _customerServiceMock.Setup(x => x.UpdateMonthlyAmountAsync(It.IsAny<int>(), It.IsAny<decimal>())).ThrowsAsync(new CustomException("O novo valor de aporte deve ser diferente do valor atual."));

            Func<Task> act = async () => await _controller.UpdateAmount(1, 300);

            await act.Should().ThrowAsync<CustomException>();
        }

        [Fact]
        public async Task GetPortfolio_ShouldReturnOk_WhenExists()
        {
            // 1. Cria o objeto de métricas (Resumo)
            var resumo = new PortfolioMetrics
            (
                50000m,
                55000m,
                5000m,
                10m
            );

            // 2. Cria a lista de ativos (Ativos)
            var ativos = new List<AssetDto>
            {
                new AssetDto ("PETR4",100,35.50m,3550m,10,20,30,40),
                new AssetDto ("VALE3",50,70.00m,3500m,10,20,30,40)
            };

            // 3. MONTA O RESPONSE (O segredo é a ordem dos parênteses)
            var response = new PortfolioSummaryResponse(
                1,                      // ClienteId
                "Seu Nome Porra",       // Nome
                "0001-9",               // ContaGrafica
                DateTime.Now,           // DataConsulta
                resumo,                 // PortfolioMetrics Resumo
                ativos                  // List<AssetDto> Ativos
            );

            _customerServiceMock.Setup(x => x.GetPortfolioSummaryAsync(It.IsAny<int>())).ReturnsAsync(response);

            var result = await _controller.GetPortfolio(1);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetPortfolio_ShouldThrow_WhenNotFound()
        {
            _customerServiceMock.Setup(x => x.GetPortfolioSummaryAsync(It.IsAny<int>())).ThrowsAsync(new CustomException("Cliente nao encontrado."));

            Func<Task> act = async () => await _controller.GetPortfolio(999);

            await act.Should().ThrowAsync<CustomException>();
        }

        [Fact]
        public async Task GetProfitability_ShouldReturnOk_WhenExists()
        {
            var metrics = new PortfolioMetrics
            (
                10000m,
                12000m,
                 2000m,
                 20m
            );

            var historico = new List<AporteDto>();
            var evolucao = new List<EvolucaoDto>();

            var response = new PortfolioProfitabilityResponse(
                1,              
                "João Silva",   
                DateTime.Now,   
                metrics,        
                historico,      
                evolucao       
            );

            _customerServiceMock.Setup(x => x.GetDetailedProfitabilityAsync(It.IsAny<int>())).ReturnsAsync(response);

            var result = await _controller.GetProfitability(1);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetProfitability_ShouldThrow_WhenNotFound()
        {
            _customerServiceMock.Setup(x => x.GetDetailedProfitabilityAsync(It.IsAny<int>())).ThrowsAsync(new CustomException("Cliente nao encontrado."));

            Func<Task> act = async () => await _controller.GetProfitability(999);

            await act.Should().ThrowAsync<CustomException>();
        }
    }
}

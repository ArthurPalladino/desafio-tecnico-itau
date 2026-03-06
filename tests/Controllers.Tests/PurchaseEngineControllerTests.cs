using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Controllers.Tests
{
    public class PurchaseEngineControllerTests
    {
        private readonly Mock<IParserB3CotHist> _parserMock;
        private readonly Mock<IPurchaseEngineService> _engineServiceMock;
        private readonly PurchaseEngineController _controller;

        public PurchaseEngineControllerTests()
        {
            _parserMock = new Mock<IParserB3CotHist>();
            _engineServiceMock = new Mock<IPurchaseEngineService>();
            _controller = new PurchaseEngineController(_engineServiceMock.Object);
        }

        [Fact]
        public async Task MotorExecute_ShouldReturnOk_WhenDataIsNominal()
        {
            var referDate = new DateTime(2026, 3, 5);
            var response = new MotorCompraResponseDto
            {
                DataExecucao = referDate,
                TotalClientes = 2,
                TotalConsolidado = 300,
                EventosIRPublicados = 10,
                Mensagem = "Compra executada",
                OrdensCompra = new List<OrdemResumoDto>(),
                Distribuicoes = new List<DistribuicaoAgrupadaDto>(),
                ResiduosCustMaster = new List<ResiduoMasterDto>()
            };

            _parserMock.Setup(x => x.ParseAndSyncDatabaseAsync()).Returns(Task.CompletedTask);
            _engineServiceMock.Setup(x => x.ExecuteAsync(referDate)).ReturnsAsync(response);

            var result = await _controller.MotorExecute(referDate);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
        }

        

        [Fact]
        public async Task MotorExecute_ShouldThrow_WhenNoActiveCustomers()
        {
            var referDate = new DateTime(2026, 3, 5);

            _parserMock.Setup(x => x.ParseAndSyncDatabaseAsync()).Returns(Task.CompletedTask);
            _engineServiceMock.Setup(x => x.ExecuteAsync(referDate)).ThrowsAsync(new CustomException("Nenhum cliente ativo encontrado."));

            Func<Task> act = async () => await _controller.MotorExecute(referDate);

            await act.Should().ThrowAsync<CustomException>();
        }

        [Fact]
        public async Task MotorExecute_ShouldThrow_WhenNoBasket()
        {
            var referDate = new DateTime(2026, 3, 5);

            _parserMock.Setup(x => x.ParseAndSyncDatabaseAsync()).Returns(Task.CompletedTask);
            _engineServiceMock.Setup(x => x.ExecuteAsync(referDate)).ThrowsAsync(new CustomException("Nenhuma cesta ativa encontrada."));

            Func<Task> act = async () => await _controller.MotorExecute(referDate);

            await act.Should().ThrowAsync<CustomException>();
        }

        [Fact]
        public async Task MotorExecute_ShouldThrow_WhenNoQuotations()
        {
            var referDate = new DateTime(2026, 3, 5);

            _parserMock.Setup(x => x.ParseAndSyncDatabaseAsync()).Returns(Task.CompletedTask);
            _engineServiceMock.Setup(x => x.ExecuteAsync(referDate)).ThrowsAsync(new CustomException("Arquivo COTAHIST nao encontrado para a data."));

            Func<Task> act = async () => await _controller.MotorExecute(referDate);

            await act.Should().ThrowAsync<CustomException>();
        }
    }
}

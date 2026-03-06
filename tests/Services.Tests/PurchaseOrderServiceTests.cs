using Xunit;
using Moq;
using FluentAssertions;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Services.Tests
{
    public class PurchaseOrderServiceTests
    {
        private readonly Mock<IPurchaseOrderRepository> _purchaseOrderRepoMock;
        private readonly PurchaseOrderService _service;

        public PurchaseOrderServiceTests()
        {
            _purchaseOrderRepoMock = new Mock<IPurchaseOrderRepository>();
            _service = new PurchaseOrderService(_purchaseOrderRepoMock.Object);
        }

        [Fact]
        public async Task CreatePurchaseOrder_ShouldCreateBothOrders_WhenQuantityIsNotRound()
        {
            _purchaseOrderRepoMock.Setup(x => x.AddAsync(It.IsAny<PurchaseOrder>())).Returns(Task.CompletedTask);

            var result = await _service.CreatePurchaseOrder(1, "PETR4", 150, 10m);

            result.Should().HaveCount(2);
            result[0].Quantity.Should().Be(50);
            result[0].MarketType.Should().Be(MarketType.Fractional);
            result[1].Quantity.Should().Be(100);
            result[1].MarketType.Should().Be(MarketType.StandardLot);
        }

        [Fact]
        public async Task CreatePurchaseOrder_ShouldCreateOnlyStandard_WhenQuantityIsRound()
        {
            _purchaseOrderRepoMock.Setup(x => x.AddAsync(It.IsAny<PurchaseOrder>())).Returns(Task.CompletedTask);

            var result = await _service.CreatePurchaseOrder(1, "PETR4", 100, 10m);

            result.Should().HaveCount(1);
            result[0].Quantity.Should().Be(100);
            result[0].MarketType.Should().Be(MarketType.StandardLot);
        }

        [Fact]
        public async Task CreatePurchaseOrder_ShouldCreateOnlyFractional_WhenQuantityBelowRound()
        {
            _purchaseOrderRepoMock.Setup(x => x.AddAsync(It.IsAny<PurchaseOrder>())).Returns(Task.CompletedTask);

            var result = await _service.CreatePurchaseOrder(1, "PETR4", 50, 10m);

            result.Should().HaveCount(1);
            result[0].Quantity.Should().Be(50);
            result[0].MarketType.Should().Be(MarketType.Fractional);
        }

        [Fact]
        public async Task CreateSellOrders_ShouldCreateOrders()
        {
            var account = new TradingAccount(null, AccountType.SubAccount);
            account.GetType().GetProperty("Id")?.SetValue(account, 1);
            var custody = new Custody(1, 1, "PETR4", 150, 10m);

            _purchaseOrderRepoMock.Setup(x => x.AddAsync(It.IsAny<PurchaseOrder>())).Returns(Task.CompletedTask);

            await _service.CreateSellOrders(account, custody, 15m);

            _purchaseOrderRepoMock.Verify(x => x.AddAsync(It.IsAny<PurchaseOrder>()), Times.AtLeast(1));
        }
    }
}

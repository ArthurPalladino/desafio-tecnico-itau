using Xunit;
using Moq;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using System.Linq;

namespace Tests.Services.Tests
{
    public class PurchaseEngineServiceTests
    {
        private readonly Mock<ICustomerRepository> _customerRepoMock;
        private readonly Mock<ITaxEventRepository> _taxEventRepoMock;
        private readonly Mock<IDistributionRepository> _distributionRepoMock;
        private readonly Mock<IRecommendationBasketRepository> _basketRepoMock;
        private readonly Mock<ITickerRepository> _tickerRepoMock;
        private readonly Mock<ITradingAccountRepository> _accountRepoMock;
        private readonly Mock<IPurchaseOrderRepository> _purchaseOrderRepoMock;
        private readonly Mock<ITaxService> _taxServiceMock;
        private readonly Mock<IPurchaseOrderService> _purchaseOrderServiceMock;
        private readonly Mock<IRebalancingEngineService> _rebalancingEngineServiceMock;
        private readonly Mock<IKafkaProducer> _kafkaProducerMock;

        private readonly PurchaseEngineService _service;

        public PurchaseEngineServiceTests()
        {
            _customerRepoMock = new Mock<ICustomerRepository>();
            _taxEventRepoMock = new Mock<ITaxEventRepository>();
            _distributionRepoMock = new Mock<IDistributionRepository>();
            _basketRepoMock = new Mock<IRecommendationBasketRepository>();
            _tickerRepoMock = new Mock<ITickerRepository>();
            _accountRepoMock = new Mock<ITradingAccountRepository>();
            _purchaseOrderRepoMock = new Mock<IPurchaseOrderRepository>();
            _taxServiceMock = new Mock<ITaxService>();
            _purchaseOrderServiceMock = new Mock<IPurchaseOrderService>();
            _rebalancingEngineServiceMock = new Mock<IRebalancingEngineService>();
            _kafkaProducerMock = new Mock<IKafkaProducer>();

            _service = new PurchaseEngineService(
                _purchaseOrderRepoMock.Object,
                _customerRepoMock.Object,
                _basketRepoMock.Object,
                _tickerRepoMock.Object,
                _accountRepoMock.Object,
                _taxServiceMock.Object,
                _distributionRepoMock.Object,
                _kafkaProducerMock.Object,
                _taxEventRepoMock.Object,
                _purchaseOrderServiceMock.Object,
                _rebalancingEngineServiceMock.Object
            );
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnValidDto_WhenPathIsNominal()
        {
            var referDate = new DateTime(2026, 2, 27);

            var masterAccount = new TradingAccount(null, AccountType.Master);
            masterAccount.GetType().GetProperty("Id")?.SetValue(masterAccount, 3);
            masterAccount.GetType().GetProperty("CustomerId")?.SetValue(masterAccount, 999);
            masterAccount.CreditBalance(100);
            _accountRepoMock.Setup(x => x.GetMasterAccount()).ReturnsAsync(masterAccount);

            var customer1 = new Customer("Customer1", "297.830.130-92", "email1@test.com", 300);
            customer1.GetType().GetProperty("Id")?.SetValue(customer1, 1);
            var tradingAccount1 = new TradingAccount(customer1, AccountType.SubAccount);
            tradingAccount1.GetType().GetProperty("Id")?.SetValue(tradingAccount1, 1);
            tradingAccount1.GetType().GetProperty("CustomerId")?.SetValue(tradingAccount1, 1);
            customer1.GetType().GetProperty("TradingAccount")?.SetValue(customer1, tradingAccount1);

            var customer2 = new Customer("Customer2", "025.390.380-77", "email2@test.com", 600);
            customer2.GetType().GetProperty("Id")?.SetValue(customer2, 2);
            var tradingAccount2 = new TradingAccount(customer2, AccountType.SubAccount);
            tradingAccount2.GetType().GetProperty("Id")?.SetValue(tradingAccount2, 2);
            tradingAccount2.GetType().GetProperty("CustomerId")?.SetValue(tradingAccount2, 2);
            customer2.GetType().GetProperty("TradingAccount")?.SetValue(customer2, tradingAccount2);

            var customers = new List<Customer> { customer1, customer2 };
            _customerRepoMock.Setup(x => x.GetActiveCustomers()).ReturnsAsync(customers);
            _customerRepoMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            var basket = new RecommendationBasket("Top 5", new List<BasketItem> { new BasketItem("ITUB4", 20m), new BasketItem("VALE3", 20m), new BasketItem("PETR4", 20m), new BasketItem("B3SA3", 20m), new BasketItem("ABEV3", 20m) });
            _basketRepoMock.Setup(x => x.GetActiveBasketWithItensAsync()).ReturnsAsync(basket);

            var ticker1 = new Ticker("PETR4", 10, referDate);
            var ticker2 = new Ticker("VALE3", 20, referDate);
            var ticker3 = new Ticker("ITUB4", 20, referDate);
            var ticker4 = new Ticker("B3SA3", 20, referDate);
            var ticker5 = new Ticker("ABEV3", 20, referDate);
            var tickers = new Dictionary<string, Ticker> { { "PETR4", ticker1 }, { "VALE3", ticker2 }, { "ITUB4", ticker3 }, { "B3SA3", ticker4 }, { "ABEV3", ticker5 } };
            _tickerRepoMock.Setup(x => x.GetTickersDictBySymbol(It.IsAny<List<string>>())).ReturnsAsync(tickers);

            var purchaseOrder1 = new PurchaseOrder(3, "PETR4", MarketType.StandardLot, 5, 10);
            purchaseOrder1.GetType().GetProperty("Id")?.SetValue(purchaseOrder1, 1);
            purchaseOrder1.GetType().GetProperty("ExecutionDate")?.SetValue(purchaseOrder1, referDate);
            
            var purchaseOrder2 = new PurchaseOrder(3, "PETR4F", MarketType.Fractional, 5, 10);
            purchaseOrder2.GetType().GetProperty("Id")?.SetValue(purchaseOrder2, 2);
            purchaseOrder2.GetType().GetProperty("ExecutionDate")?.SetValue(purchaseOrder2, referDate);

            _taxServiceMock.Setup(x => x.CalculateRetentionTax(It.IsAny<decimal>())).Returns(0.01m);
            _taxServiceMock.Setup(x => x.PostTaxInKafkaAsync(It.IsAny<Customer>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<TaxType>())).Returns(Task.CompletedTask);
            _distributionRepoMock.Setup(x => x.AddAsync(It.IsAny<Distribution>())).Returns(Task.CompletedTask);
            _rebalancingEngineServiceMock.Setup(x => x.ExecuteAsync(RebalancingType.OutofBalance)).ReturnsAsync(true);

            _purchaseOrderServiceMock
            .Setup(x => x.CreatePurchaseOrder(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()))
            .ReturnsAsync((int accId, string sym, int qty, decimal price) =>
                new List<PurchaseOrder> {
                    new PurchaseOrder(accId, sym, MarketType.StandardLot, qty > 0 ? qty : 1, price)
                });

            _purchaseOrderServiceMock
                .Setup(x => x.CreatePurchaseOrder(3, "PETR4", It.IsAny<int>(), It.IsAny<decimal>()))
                .ReturnsAsync(new List<PurchaseOrder> { purchaseOrder1, purchaseOrder2 });

            _purchaseOrderRepoMock.Setup(x => x.GetLastOrderAsync(It.IsAny<string>())).ReturnsAsync(purchaseOrder1);

            var result = await _service.ExecuteAsync(referDate);

            result.Should().NotBeNull();
            result.DataExecucao.Should().Be(referDate);
            result.TotalClientes.Should().Be(2);
            result.TotalConsolidado.Should().Be(300);
            result.Mensagem.Should().Contain("Compra programada executada com sucesso para 2 clientes");
            result.EventosIRPublicados.Should().BeGreaterThan(0);
            result.OrdensCompra.Should().NotBeEmpty();
            result.Distribuicoes.Should().HaveCount(2);

            _customerRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _rebalancingEngineServiceMock.Verify(x => x.ExecuteAsync(RebalancingType.OutofBalance), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowException_WhenBasketIsNotFound()
        {
            var referDate = new DateTime(2026, 3, 5);
            var customers = new List<Customer> { new Customer("Customer1", "297.830.130-92", "email1@test.com", 300) };
            _customerRepoMock.Setup(x => x.GetActiveCustomers()).ReturnsAsync(customers);
            _basketRepoMock.Setup(x => x.GetActiveBasketWithItensAsync()).ReturnsAsync((RecommendationBasket?)null);

            Func<Task> act = async () => await _service.ExecuteAsync(referDate);

            await act.Should().ThrowAsync<CustomException>().WithMessage("Nenhuma cesta ativa encontrada.");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowException_WhenTickersAreMissing()
        {
            var referDate = new DateTime(2026, 3, 5);
            var customers = new List<Customer> { new Customer("Customer1", "297.830.130-92", "email1@test.com", 300) };
            _customerRepoMock.Setup(x => x.GetActiveCustomers()).ReturnsAsync(customers);
            var basket = new RecommendationBasket("Top 5", new List<BasketItem> { new BasketItem("ITUB4", 20m), new BasketItem("VALE3", 20m), new BasketItem("PETR4", 20m), new BasketItem("B3SA3", 20m), new BasketItem("ABEV3", 20m) });
            _basketRepoMock.Setup(x => x.GetActiveBasketWithItensAsync()).ReturnsAsync(basket);
            _tickerRepoMock.Setup(x => x.GetTickersDictBySymbol(It.IsAny<List<string>>())).ReturnsAsync((Dictionary<string, Ticker>?)null);

            Func<Task> act = async () => await _service.ExecuteAsync(referDate);

            await act.Should().ThrowAsync<CustomException>().WithMessage("Arquivo COTAHIST nao encontrado para a data.");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldProcessWithoutPurchases_WhenCustomerHasNoBalance()
        {
            var referDate = new DateTime(2026, 2, 27);

            var customer = new Customer("Customer1", "297.830.130-92", "email1@test.com", 100);
            customer.GetType().GetProperty("Id")?.SetValue(customer, 1);
            var tradingAccount = new TradingAccount(customer, AccountType.SubAccount);
            tradingAccount.GetType().GetProperty("Id")?.SetValue(tradingAccount, 1);
            tradingAccount.GetType().GetProperty("CustomerId")?.SetValue(tradingAccount, 1);
            customer.GetType().GetProperty("TradingAccount")?.SetValue(customer, tradingAccount);

            var customers = new List<Customer> { customer };
            _customerRepoMock.Setup(x => x.GetActiveCustomers()).ReturnsAsync(customers);
            _customerRepoMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            var basket = new RecommendationBasket("Top 5", new List<BasketItem>
            {
                new BasketItem("ITUB4", 20m),
                new BasketItem("VALE3", 20m),
                new BasketItem("PETR4", 20m),
                new BasketItem("B3SA3", 20m),
                new BasketItem("ABEV3", 20m)
            });
            _basketRepoMock.Setup(x => x.GetActiveBasketWithItensAsync()).ReturnsAsync(basket);

            var tickers = new Dictionary<string, Ticker>
            {
                { "ITUB4", new Ticker("ITUB4", 1000m, referDate) },
                { "VALE3", new Ticker("VALE3", 1000m, referDate) },
                { "PETR4", new Ticker("PETR4", 1000m, referDate) },
                { "B3SA3", new Ticker("B3SA3", 1000m, referDate) },
                { "ABEV3", new Ticker("ABEV3", 1000m, referDate) }
            };
            _tickerRepoMock.Setup(x => x.GetTickersDictBySymbol(It.IsAny<List<string>>())).ReturnsAsync(tickers);

            var masterAccount = new TradingAccount(null, AccountType.Master);
            masterAccount.GetType().GetProperty("Id")?.SetValue(masterAccount, 3);
            masterAccount.GetType().GetProperty("CustomerId")?.SetValue(masterAccount, 999);
            _accountRepoMock.Setup(x => x.GetMasterAccount()).ReturnsAsync(masterAccount);

            _rebalancingEngineServiceMock.Setup(x => x.ExecuteAsync(RebalancingType.OutofBalance)).ReturnsAsync(true);

            var result = await _service.ExecuteAsync(referDate);

            result.Should().NotBeNull();
            result.OrdensCompra.Should().BeEmpty();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCalculateRoundingAndResidue_Correctly()
        {
            var referDate = new DateTime(2026, 3, 5);

            var customer = new Customer("Customer1", "297.830.130-92", "email1@test.com", 100);
            customer.GetType().GetProperty("Id")?.SetValue(customer, 1);
            var tradingAccount = new TradingAccount(customer, AccountType.SubAccount);
            tradingAccount.GetType().GetProperty("Id")?.SetValue(tradingAccount, 1);
            tradingAccount.GetType().GetProperty("CustomerId")?.SetValue(tradingAccount, 1);
            customer.GetType().GetProperty("TradingAccount")?.SetValue(customer, tradingAccount);

            var customers = new List<Customer> { customer };
            _customerRepoMock.Setup(x => x.GetActiveCustomers()).ReturnsAsync(customers);

            var basket = new RecommendationBasket("Top 5", new List<BasketItem> { new BasketItem("ITUB4", 20m), new BasketItem("VALE3", 20m), new BasketItem("PETR4", 20m), new BasketItem("B3SA3", 20m), new BasketItem("ABEV3", 20m) });
            _basketRepoMock.Setup(x => x.GetActiveBasketWithItensAsync()).ReturnsAsync(basket);

            var tickers = new Dictionary<string, Ticker> 
            { 
                { "PETR4", new Ticker("PETR4", 10m, referDate) },
                { "VALE3", new Ticker("VALE3", 20m, referDate) },
                { "ITUB4", new Ticker("ITUB4", 20m, referDate) },
                { "B3SA3", new Ticker("B3SA3", 20m, referDate) },
                { "ABEV3", new Ticker("ABEV3", 10m, referDate) }
            };
            _tickerRepoMock.Setup(x => x.GetTickersDictBySymbol(It.IsAny<List<string>>())).ReturnsAsync(tickers);

            var masterAccount = new TradingAccount(null, AccountType.Master);
            masterAccount.GetType().GetProperty("Id")?.SetValue(masterAccount, 3);
            masterAccount.GetType().GetProperty("CustomerId")?.SetValue(masterAccount, 999);
            foreach (var t in tickers.Values) { masterAccount.AddCustody(t.Symbol, 100, t.CurrentPrice); }
            _accountRepoMock.Setup(x => x.GetMasterAccount()).ReturnsAsync(masterAccount);

            _purchaseOrderServiceMock.Setup(x => x.CreatePurchaseOrder(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()))
                .ReturnsAsync((int accId, string sym, int qty, decimal price) => 
                {
                    var order = new PurchaseOrder(accId, sym, MarketType.StandardLot, qty > 0 ? qty : 1, price);
                    order.GetType().GetProperty("Id")?.SetValue(order, 1);
                    order.GetType().GetProperty("ExecutionDate")?.SetValue(order, referDate);
                    return new List<PurchaseOrder> { order };
                });

            _purchaseOrderRepoMock.Setup(x => x.GetLastOrderAsync(It.IsAny<string>()))
                .ReturnsAsync((string sym) => {
                    var o = new PurchaseOrder(3, sym, MarketType.StandardLot, 1, 10m);
                    o.GetType().GetProperty("Id")?.SetValue(o, 1);
                    o.GetType().GetProperty("ExecutionDate")?.SetValue(o, referDate);
                    return o;
                });

            _rebalancingEngineServiceMock.Setup(x => x.ExecuteAsync(RebalancingType.OutofBalance)).ReturnsAsync(true);

            var result = await _service.ExecuteAsync(referDate);

            result.Should().NotBeNull();
            result.ResiduosCustMaster.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowException_WhenNoActiveCustomersFound()
        {
            var referDate = new DateTime(2026, 3, 5);
            _customerRepoMock.Setup(x => x.GetActiveCustomers()).ReturnsAsync(new List<Customer>());

            Func<Task> act = async () => await _service.ExecuteAsync(referDate);

            await act.Should().ThrowAsync<CustomException>().WithMessage("Nenhum cliente ativo encontrado.");
        }
    }
}
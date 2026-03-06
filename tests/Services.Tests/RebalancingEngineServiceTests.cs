using System;
using Xunit;
using Moq;
using FluentAssertions;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItauTopFive.Tests.Services
{
    public class RebalancingEngineServiceTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<ITaxEventRepository> _taxEventRepositoryMock;
        private readonly Mock<IDistributionRepository> _distributionRepositoryMock;
        private readonly Mock<IRecommendationBasketRepository> _basketRepositoryMock;
        private readonly Mock<ITickerRepository> _tickerRepositoryMock;
        private readonly Mock<ITradingAccountRepository> _accountRepositoryMock;
        private readonly Mock<IPurchaseOrderRepository> _purchaseOrderRepositoryMock;
        private readonly Mock<ITaxService> _taxServiceMock;
        private readonly Mock<IKafkaProducer> _kafkaProducerMock;
        private readonly Mock<IPurchaseOrderService> _purchaseOrderServiceMock;
        private readonly RebalancingEngineService _rebalancingEngineService;

        public RebalancingEngineServiceTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _taxEventRepositoryMock = new Mock<ITaxEventRepository>();
            _distributionRepositoryMock = new Mock<IDistributionRepository>();
            _basketRepositoryMock = new Mock<IRecommendationBasketRepository>();
            _tickerRepositoryMock = new Mock<ITickerRepository>();
            _accountRepositoryMock = new Mock<ITradingAccountRepository>();
            _purchaseOrderRepositoryMock = new Mock<IPurchaseOrderRepository>();
            _taxServiceMock = new Mock<ITaxService>();
            _kafkaProducerMock = new Mock<IKafkaProducer>();
            _purchaseOrderServiceMock = new Mock<IPurchaseOrderService>();

            _rebalancingEngineService = new RebalancingEngineService(
                _purchaseOrderRepositoryMock.Object,
                _customerRepositoryMock.Object,
                _basketRepositoryMock.Object,
                _tickerRepositoryMock.Object,
                _accountRepositoryMock.Object,
                _taxServiceMock.Object,
                _distributionRepositoryMock.Object,
                _kafkaProducerMock.Object,
                _taxEventRepositoryMock.Object,
                _purchaseOrderServiceMock.Object
            );
        }

        #region Helper Methods

        private Customer CreateCustomerWithAccount(int id, string name, decimal balance, List<Custody> custodies)
        {
            var customer = new Customer(name, "78674491081", "test@example.com", 1000);
            typeof(Customer).GetProperty("Id")?.SetValue(customer, id);

            var account = new TradingAccount(customer, AccountType.SubAccount);
            typeof(TradingAccount).GetProperty("Id")?.SetValue(account, id);
            typeof(TradingAccount).GetProperty("Balance")?.SetValue(account, balance);
            typeof(TradingAccount).GetProperty("CustomerId")?.SetValue(account, id);
            foreach (var custody in custodies)
            {
                account.Custodies.Add(custody);
            }

            typeof(Customer).GetProperty("TradingAccount")?.SetValue(customer, account);
            return customer;
        }

        private TradingAccount CreateMasterAccount(int id, decimal balance)
        {
            var master = new Customer("Master", "34619554006", "master@broker.com", 100);
            var account = new TradingAccount(master, AccountType.Master);
            typeof(TradingAccount).GetProperty("Id")?.SetValue(account, id);
            typeof(TradingAccount).GetProperty("Balance")?.SetValue(account, balance);
            return account;
        }

        private RecommendationBasket CreateBasket()
        {
            var items = new List<BasketItem>
            {
                new BasketItem("PETR4", 20m),
                new BasketItem("VALE3", 20m),
                new BasketItem("ITUB4", 20m),
                new BasketItem("BBDC4", 20m),
                new BasketItem("ABEV3", 20m)
            };
            return new RecommendationBasket("Top 5", items);
        }

        private Dictionary<string, Ticker> CreateTickers()
        {
            return new Dictionary<string, Ticker>
            {
                { "PETR4", new Ticker("PETR4", 25m, DateTime.Now) },
                { "VALE3", new Ticker("VALE3", 30m, DateTime.Now) },
                { "ITUB4", new Ticker("ITUB4", 40m, DateTime.Now) },
                { "BBDC4", new Ticker("BBDC4", 35m, DateTime.Now) },
                { "ABEV3", new Ticker("ABEV3", 15m, DateTime.Now) },
                { "OLDSTK", new Ticker("OLDSTK", 10m, DateTime.Now) }
            };
        }

        #endregion

        [Fact]
        public async Task ExecuteAsync_ShouldSellAsset_WhenWeightAboveThreshold()
        {
            var custodies = new List<Custody> { new Custody(1, 1, "PETR4", 100, 20m) };
            var customer = CreateCustomerWithAccount(1, "User 1", 5000, custodies);
            var tickers = CreateTickers();
            var basket = CreateBasket();

            _customerRepositoryMock.Setup(x => x.GetActiveCustomers()).ReturnsAsync(new List<Customer> { customer });
            _basketRepositoryMock.Setup(x => x.GetActiveBasketWithItensAsync()).ReturnsAsync(basket);
            _tickerRepositoryMock.Setup(x => x.GetTickersDictByLastDate()).ReturnsAsync(tickers);
            _accountRepositoryMock.Setup(x => x.GetMasterAccount()).ReturnsAsync(CreateMasterAccount(999, 0));
            _purchaseOrderRepositoryMock.Setup(x => x.GetTotalSalesValueInMonthAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(0m);
            _taxServiceMock.Setup(x => x.CalculateCapitalGainTax(It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<decimal>())).Returns(10m);
            _purchaseOrderServiceMock.Setup(x => x.CreateSellOrders(It.IsAny<TradingAccount>(), It.IsAny<Custody>(), It.IsAny<decimal>())).Returns(Task.CompletedTask);
            _taxServiceMock.Setup(x => x.PostTaxInKafkaAsync(It.IsAny<Customer>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<TaxType>())).Returns(Task.CompletedTask);

            var result = await _rebalancingEngineService.ExecuteAsync(RebalancingType.OutofBalance);

            result.Should().BeTrue();
            _purchaseOrderServiceMock.Verify(x => x.CreateSellOrders(It.IsAny<TradingAccount>(), It.IsAny<Custody>(), It.IsAny<decimal>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldBuyAsset_WhenBelowTargetWithBalance()
        {
            var custodies = new List<Custody> { new Custody(2, 2, "ITUB4", 10, 30m) };
            var customer = CreateCustomerWithAccount(2, "User 2", 2000, custodies);
            var tickers = CreateTickers();
            var basket = CreateBasket();

            _customerRepositoryMock.Setup(x => x.GetActiveCustomers()).ReturnsAsync(new List<Customer> { customer });
            _basketRepositoryMock.Setup(x => x.GetActiveBasketWithItensAsync()).ReturnsAsync(basket);
            _tickerRepositoryMock.Setup(x => x.GetTickersDictByLastDate()).ReturnsAsync(tickers);
            _accountRepositoryMock.Setup(x => x.GetMasterAccount()).ReturnsAsync(CreateMasterAccount(999, 0));
            _purchaseOrderServiceMock.Setup(x => x.CreatePurchaseOrder(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>())).ReturnsAsync(new List<PurchaseOrder>());
            _taxServiceMock.Setup(x => x.CalculateRetentionTax(It.IsAny<decimal>())).Returns(1m);
            _taxServiceMock.Setup(x => x.PostTaxInKafkaAsync(It.IsAny<Customer>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<TaxType>())).Returns(Task.CompletedTask);

            var result = await _rebalancingEngineService.ExecuteAsync(RebalancingType.OutofBalance);

            result.Should().BeTrue();
            _purchaseOrderServiceMock.Verify(x => x.CreatePurchaseOrder(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldTransferResidualToMaster()
        {
            var customer = CreateCustomerWithAccount(3, "User 3", 50, new List<Custody>());
            var master = CreateMasterAccount(999, 100);
            var tickers = CreateTickers();
            var basket = CreateBasket();

            _customerRepositoryMock.Setup(x => x.GetActiveCustomers()).ReturnsAsync(new List<Customer> { customer });
            _basketRepositoryMock.Setup(x => x.GetActiveBasketWithItensAsync()).ReturnsAsync(basket);
            _tickerRepositoryMock.Setup(x => x.GetTickersDictByLastDate()).ReturnsAsync(tickers);
            _accountRepositoryMock.Setup(x => x.GetMasterAccount()).ReturnsAsync(master);

            await _rebalancingEngineService.ExecuteAsync(RebalancingType.OutofBalance);

            customer.TradingAccount.Balance.Should().Be(0);
            master.Balance.Should().Be(150);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowException_WhenBasketNull()
        {
            var customer = CreateCustomerWithAccount(4, "User 4", 1000, new List<Custody>());
            _customerRepositoryMock.Setup(x => x.GetActiveCustomers()).ReturnsAsync(new List<Customer> { customer });
            _basketRepositoryMock.Setup(x => x.GetActiveBasketWithItensAsync()).ReturnsAsync((RecommendationBasket?)null);

            Func<Task> act = async () => await _rebalancingEngineService.ExecuteAsync(RebalancingType.OutofBalance);

            await act.Should().ThrowAsync<CustomException>()
        .WithMessage("Nenhuma cesta ativa encontrada.");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnFalse_WhenNoCustomers()
        {
            _customerRepositoryMock.Setup(x => x.GetActiveCustomers()).ReturnsAsync(new List<Customer>());

            var result = await _rebalancingEngineService.ExecuteAsync(RebalancingType.OutofBalance);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExecuteAsync_MasterRebalancing_ShouldSellAssetsNotInBasket()
        {
            var master = CreateMasterAccount(999, 0);
            master.Custodies.Add(new Custody(999, 999, "OLDSTK", 100, 10m));

            var customer = CreateCustomerWithAccount(5, "User 5", 0, new List<Custody>());
            var basket = CreateBasket();
            var tickers = CreateTickers();

            _customerRepositoryMock.Setup(x => x.GetActiveCustomers()).ReturnsAsync(new List<Customer> { customer });
            _basketRepositoryMock.Setup(x => x.GetActiveBasketWithItensAsync()).ReturnsAsync(basket);
            _tickerRepositoryMock.Setup(x => x.GetTickersDictByLastDate()).ReturnsAsync(tickers);
            _accountRepositoryMock.Setup(x => x.GetMasterAccount()).ReturnsAsync(master);
            _purchaseOrderServiceMock.Setup(x => x.CreateSellOrders(It.IsAny<TradingAccount>(), It.IsAny<Custody>(), It.IsAny<decimal>())).Returns(Task.CompletedTask);

            await _rebalancingEngineService.ExecuteAsync(RebalancingType.RecommendationChange);

            _purchaseOrderServiceMock.Verify(x => x.CreateSellOrders(It.Is<TradingAccount>(a => a.Id == 999), It.Is<Custody>(c => c.Symbol == "OLDSTK"), It.IsAny<decimal>()), Times.Once);
        }
    }
}
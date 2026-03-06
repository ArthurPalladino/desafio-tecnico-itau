using Xunit;
using Moq;
using FluentAssertions;
using Repositories.Interfaces;

namespace Tests.Services.Tests
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _customerRepoMock;
        private readonly Mock<ITradingAccountRepository> _tradingAccountRepoMock;
        private readonly Mock<IContributionHistoryRepository> _historyRepoMock;
        private readonly Mock<ITickerRepository> _tickerRepoMock;
        private readonly Mock<IPurchaseOrderRepository> _purchaseRepoMock;
        private readonly Mock<IDistributionRepository> _distributionRepoMock;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            _customerRepoMock = new Mock<ICustomerRepository>();
            _tradingAccountRepoMock = new Mock<ITradingAccountRepository>();
            _historyRepoMock = new Mock<IContributionHistoryRepository>();
            _tickerRepoMock = new Mock<ITickerRepository>();
            _purchaseRepoMock = new Mock<IPurchaseOrderRepository>();
            _distributionRepoMock = new Mock<IDistributionRepository>();

            _service = new CustomerService(
                _customerRepoMock.Object,
                _tradingAccountRepoMock.Object,
                _historyRepoMock.Object,
                _tickerRepoMock.Object,
                _purchaseRepoMock.Object,
                _distributionRepoMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateCustomer_WhenDataIsNominal()
        {
            var request = new CreateCustomerRequest
            (
                 "TestCustomer",
                 "12345678909",
                 "test@example.com",
                 300
            );

            _customerRepoMock.Setup(x => x.GetCustomerByCpf(It.IsAny<string>())).ReturnsAsync((Customer?)null);
            _historyRepoMock.Setup(x => x.AddAsync(It.IsAny<ContributionHistory>())).Returns(Task.CompletedTask);
            _customerRepoMock.Setup(x => x.AddAsync(It.IsAny<Customer>())).Returns(Task.CompletedTask);
            _tradingAccountRepoMock.Setup(x => x.AddAsync(It.IsAny<TradingAccount>())).Returns(Task.CompletedTask);
            _customerRepoMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _service.CreateAsync(request);

            result.Should().NotBeNull();
            result.Nome.Should().Be("TestCustomer");
            result.Email.Should().Be("test@example.com");
            result.Ativo.Should().BeTrue();
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenValueTooLow()
        {
            var request = new CreateCustomerRequest
            (
                "Test",
                "12345678909",
                "test@example.com",
                50
            );

            Func<Task> act = async () => await _service.CreateAsync(request);

            await act.Should().ThrowAsync<CustomException>().WithMessage("O valor mensal minimo e de R$ 100,00.");
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenCpfDuplicate()
        {
            var request = new CreateCustomerRequest
            (
                "Test",
                "12345678909",
                "test@example.com",
                300
            );

            var existingCustomer = new Customer("Existing", "12345678909", "existing@example.com", 300);
            _customerRepoMock.Setup(x => x.GetCustomerByCpf(It.IsAny<string>())).ReturnsAsync(existingCustomer);

            Func<Task> act = async () => await _service.CreateAsync(request);

            await act.Should().ThrowAsync<CustomException>().WithMessage("CPF ja cadastrado no sistema.");
        }

        [Fact]
        public async Task LeaveInvestmentProductAsync_ShouldDeactivateCustomer_WhenExists()
        {
            var customer = new Customer("Test", "12345678909", "test@example.com", 300);
            customer.GetType().GetProperty("Id")?.SetValue(customer, 1);

            _customerRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);
            _customerRepoMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _service.LeaveInvestmentProductAsync(1);

            result.Should().NotBeNull();
            result.Ativo.Should().BeFalse();
            _customerRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task LeaveInvestmentProductAsync_ShouldThrowException_WhenNotFound()
        {
            _customerRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Customer?)null);

            Func<Task> act = async () => await _service.LeaveInvestmentProductAsync(1);

            await act.Should().ThrowAsync<CustomException>().WithMessage("Cliente nao encontrado.");
        }

        [Fact]
        public async Task LeaveInvestmentProductAsync_ShouldThrowException_WhenAlreadyInactive()
        {
            var customer = new Customer("Test", "12345678909", "test@example.com", 300);
            customer.GetType().GetProperty("Id")?.SetValue(customer, 1);
            customer.UpdateSubscriptionState(false);

            _customerRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);

            Func<Task> act = async () => await _service.LeaveInvestmentProductAsync(1);

            await act.Should().ThrowAsync<CustomException>().WithMessage("Cliente ja havia saido do produto.");
        }

        [Fact]
        public async Task UpdateMonthlyAmountAsync_ShouldUpdateValue_WhenValid()
        {
            var customer = new Customer("Test", "12345678909", "test@example.com", 300);
            customer.GetType().GetProperty("Id")?.SetValue(customer, 1);

            _customerRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);
            _historyRepoMock.Setup(x => x.AddAsync(It.IsAny<ContributionHistory>())).Returns(Task.CompletedTask);
            _customerRepoMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _service.UpdateMonthlyAmountAsync(1, 500);

            result.Should().NotBeNull();
            result.ValorMensalNovo.Should().Be(500);
            _customerRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateMonthlyAmountAsync_ShouldThrowException_WhenValueTooLow()
        {
            Func<Task> act = async () => await _service.UpdateMonthlyAmountAsync(1, 50);

            await act.Should().ThrowAsync<CustomException>().WithMessage("O valor mensal minimo e de R$ 100,00.");
        }

        [Fact]
        public async Task UpdateMonthlyAmountAsync_ShouldThrowException_WhenNotFound()
        {
            _customerRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Customer?)null);

            Func<Task> act = async () => await _service.UpdateMonthlyAmountAsync(1, 500);

            await act.Should().ThrowAsync<CustomException>().WithMessage("Cliente nao encontrado.");
        }

        [Fact]
        public async Task UpdateMonthlyAmountAsync_ShouldThrowException_WhenValueSame()
        {
            var customer = new Customer("Test", "12345678909", "test@example.com", 300);
            customer.GetType().GetProperty("Id")?.SetValue(customer, 1);

            _customerRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);

            Func<Task> act = async () => await _service.UpdateMonthlyAmountAsync(1, 300);

            await act.Should().ThrowAsync<CustomException>().WithMessage("O novo valor de aporte deve ser diferente do valor atual.");
        }

        [Fact]
        public async Task GetPortfolioSummaryAsync_ShouldReturnPortfolio_WhenExists()
        {
            var customer = new Customer("Test", "12345678909", "test@example.com", 300);
            customer.GetType().GetProperty("Id")?.SetValue(customer, 1);
            var account = new TradingAccount(customer, AccountType.SubAccount);
            account.GetType().GetProperty("Id")?.SetValue(account, 1);
            customer.GetType().GetProperty("TradingAccount")?.SetValue(customer, account);

            _customerRepoMock.Setup(x => x.GetCustomerWithPortfolioAsync(1)).ReturnsAsync(customer);
            _tickerRepoMock.Setup(x => x.GetTickersDictBySymbol(It.IsAny<List<string>>())).ReturnsAsync(new Dictionary<string, Ticker>());

            var result = await _service.GetPortfolioSummaryAsync(1);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetPortfolioSummaryAsync_ShouldThrowException_WhenNotFound()
        {
            _customerRepoMock.Setup(x => x.GetCustomerWithPortfolioAsync(It.IsAny<int>())).ReturnsAsync((Customer?)null);

            Func<Task> act = async () => await _service.GetPortfolioSummaryAsync(1);

            await act.Should().ThrowAsync<CustomException>().WithMessage("Cliente nao encontrado.");
        }
    }

}

using Xunit;
using Moq;
using FluentAssertions;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Tests.Services.Tests
{
    public class MasterAccountServiceTests
    {
        private readonly Mock<ICustomerRepository> _customerRepoMock;
        private readonly Mock<ITickerRepository> _tickerRepoMock;
        private readonly Mock<IDistributionRepository> _distributionRepoMock;
        private readonly MasterAccountService _service;

        public MasterAccountServiceTests()
        {
            _customerRepoMock = new Mock<ICustomerRepository>();
            _tickerRepoMock = new Mock<ITickerRepository>();
            _distributionRepoMock = new Mock<IDistributionRepository>();

            _service = new MasterAccountService(
                _customerRepoMock.Object,
                _tickerRepoMock.Object,
                _distributionRepoMock.Object
            );
        }

        [Fact]
        public async Task GetMasterCustodyAsync_ShouldReturnCustody_WhenExists()
        {
            var customer = new Customer("Master", "12345678909", "master@example.com", 300);
            customer.GetType().GetProperty("Id")?.SetValue(customer, 1);

            var account = new TradingAccount(customer, AccountType.Master);
            account.GetType().GetProperty("Id")?.SetValue(account, 1);
            account.GetType().GetProperty("CustomerId")?.SetValue(account, 1);
            customer.GetType().GetProperty("TradingAccount")?.SetValue(customer, account);

            account.AddCustody("PETR4", 100, 10);

            _customerRepoMock.Setup(x => x.GetCustomerWithPortfolioAsync(1)).ReturnsAsync(customer);
            _tickerRepoMock.Setup(x => x.GetTickersDictBySymbol(It.IsAny<List<string>>())).ReturnsAsync(new Dictionary<string, Ticker> { { "PETR4", new Ticker("PETR4", 15, DateTime.Now) } });
            _distributionRepoMock.Setup(x => x.GetLatestDistributionByTickerAsync(It.IsAny<string>())).ReturnsAsync((Distribution?)null);

            var result = await _service.GetMasterCustodyAsync();

            result.Should().NotBeNull();
        }
    }
    
}

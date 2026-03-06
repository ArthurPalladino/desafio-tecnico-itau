using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Data;
using System;
using System.Threading.Tasks;

namespace Tests.Repositories.Tests
{
    public class TradingAccountRepositoryTests
    {
        private ItauTopFiveDbContext CreateContext(string databaseName = null)
        {
            var options = new DbContextOptionsBuilder<ItauTopFiveDbContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;
            return new ItauTopFiveDbContext(options);
        }

        [Fact]
        public async Task GetByCustomerIdAsync_ShouldReturnAccount_WhenExists()
        {
            using (var context = CreateContext())
            {
                var customer = new Customer("Test", "29783013092", "test@example.com", 300);
                var account = new TradingAccount(customer, AccountType.SubAccount);

                context.Customers.Add(customer);
                context.TradingAccounts.Add(account);
                await context.SaveChangesAsync();

                var repo = new TradingAccountRepository(context);
                var result = await repo.GetByCustomerIdAsync(customer.Id);

                result.Should().NotBeNull();
                result.Type.Should().Be(AccountType.SubAccount);
            }
        }

        [Fact]
        public async Task GetByCustomerIdAsync_ShouldReturnNull_WhenNotExists()
        {
            using (var context = CreateContext())
            {
                var repo = new TradingAccountRepository(context);
                var result = await repo.GetByCustomerIdAsync(999);

                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetMasterAccount_ShouldReturnAccountWithId1()
        {
            using (var context = CreateContext())
            {
                var customer = new Customer("Master", "989.261.080-66", "master@example.com", 300);
                var account = new TradingAccount(customer, AccountType.Master);
                account.GetType().GetProperty("Id")?.SetValue(account, 1);

                context.Customers.Add(customer);
                context.TradingAccounts.Add(account);
                await context.SaveChangesAsync();

                var repo = new TradingAccountRepository(context);
                var result = await repo.GetMasterAccount();

                result.Should().NotBeNull();
                result.Type.Should().Be(AccountType.Master);
            }
        }

        [Fact]
        public async Task GetMasterAccount_ShouldIncludeCustodies()
        {
            using (var context = CreateContext())
            {
                var customer = new Customer("Master", "501.379.630-09", "master@example.com", 300);
                customer.GetType().GetProperty("Id")?.SetValue(customer, 1);

                var account = new TradingAccount(customer, AccountType.Master);
                account.GetType().GetProperty("Id")?.SetValue(account, 1);
                account.GetType().GetProperty("CustomerId")?.SetValue(account, 1);

                account.AddCustody("PETR4", 100, 10);

                context.Customers.Add(customer);
                context.TradingAccounts.Add(account);
                await context.SaveChangesAsync();

                var repo = new TradingAccountRepository(context);
                var result = await repo.GetMasterAccount();

                result.Should().NotBeNull();
                result.Custodies.Should().NotBeEmpty();
            }
        }

        [Fact]
        public async Task AddAsync_ShouldPersistAccount()
        {
            var dbName = Guid.NewGuid().ToString();

            using (var context = CreateContext(dbName))
            {
                var customer = new Customer("Test", "29783013092", "test@example.com", 300);
                context.Customers.Add(customer);
                await context.SaveChangesAsync();

                var account = new TradingAccount(customer, AccountType.SubAccount);
                var repo = new TradingAccountRepository(context);
                await repo.AddAsync(account);
                await repo.SaveChangesAsync();
            }

            using (var verifyContext = CreateContext(dbName))
            {
                var verifyRepo = new TradingAccountRepository(verifyContext);
                var result = await verifyRepo.GetAllAsync();
                result.Should().HaveCount(1);
            }
        }

        [Fact]
        public async Task Update_ShouldPersistChanges()
        {
            var dbName = Guid.NewGuid().ToString();
            int accountId;

            using (var context = CreateContext(dbName))
            {
                var customer = new Customer("Test", "29783013092", "test@example.com", 300);
                var account = new TradingAccount(customer, AccountType.SubAccount);

                context.Customers.Add(customer);
                context.TradingAccounts.Add(account);
                await context.SaveChangesAsync();
                accountId = account.Id;

                var repo = new TradingAccountRepository(context);
                account.CreditBalance(1000);
                repo.Update(account);
                await repo.SaveChangesAsync();
            }

            using (var verifyContext = CreateContext(dbName))
            {
                var verifyRepo = new TradingAccountRepository(verifyContext);
                var result = await verifyRepo.GetByIdAsync(accountId);
                result.Balance.Should().Be(1000);
            }
        }
    }
}
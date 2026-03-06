using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Repositories.Tests
{
    public class CustomerRepositoryTests
    {
        private ItauTopFiveDbContext CreateContext(string databaseName = null)
        {
            var options = new DbContextOptionsBuilder<ItauTopFiveDbContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;
            return new ItauTopFiveDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistEntity_WhenEntityIsValid()
        {
            var dbName = Guid.NewGuid().ToString();

            using (var context = CreateContext(dbName))
            {
                var repo = new CustomerRepository(context);
                var customer = new Customer("Test", "29783013092", "test@example.com", 300);

                await repo.AddAsync(customer);
                await repo.SaveChangesAsync();
            }

            using (var verifyContext = CreateContext(dbName))
            {
                var verifyRepo = new CustomerRepository(verifyContext);
                var result = await verifyRepo.GetAllAsync();
                result.Should().ContainSingle(c => c.Email == "test@example.com");
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEntity_WhenRecordExists()
        {
            using (var context = CreateContext())
            {
                var customer = new Customer("Test", "29783013092", "test@example.com", 300);
                context.Customers.Add(customer);
                await context.SaveChangesAsync();

                var repo = new CustomerRepository(context);
                var result = await repo.GetByIdAsync(customer.Id);

                result.Should().NotBeNull();
                result.Email.Should().Be("test@example.com");
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenRecordNotFound()
        {
            using (var context = CreateContext())
            {
                var repo = new CustomerRepository(context);
                var result = await repo.GetByIdAsync(999);

                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetCustomerByCpf_ShouldReturnEntity_WhenCpfExists()
        {
            using (var context = CreateContext())
            {
                var customer = new Customer("Test", "29783013092", "test@example.com", 300);
                context.Customers.Add(customer);
                await context.SaveChangesAsync();

                var repo = new CustomerRepository(context);
                var result = await repo.GetCustomerByCpf("29783013092");

                result.Should().NotBeNull();
                result.Name.Should().Be("Test");
            }
        }

        [Fact]
        public async Task GetCustomerByCpf_ShouldReturnNull_WhenCpfNotExists()
        {
            using (var context = CreateContext())
            {
                var repo = new CustomerRepository(context);
                var result = await repo.GetCustomerByCpf("00000000000");

                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetActiveCustomers_ShouldReturnOnlyActiveRecords_WhenCalled()
        {
            using (var context = CreateContext())
            {
                var activeCustomer = new Customer("Active", "428.771.070-35", "active@example.com", 300);
                var inactiveCustomer = new Customer("Inactive", "542.187.930-50", "inactive@example.com", 300);
                inactiveCustomer.UpdateSubscriptionState(false);

                var account1 = new TradingAccount(activeCustomer, AccountType.SubAccount);
                var account2 = new TradingAccount(inactiveCustomer, AccountType.SubAccount);

                context.Customers.AddRange(activeCustomer, inactiveCustomer);
                context.TradingAccounts.AddRange(account1, account2);
                await context.SaveChangesAsync();

                var repo = new CustomerRepository(context);
                var result = await repo.GetActiveCustomers();

                result.Should().ContainSingle(c => c.IsActive && c.Name == "Active");
            }
        }

        [Fact]
        public async Task GetActiveCustomers_ShouldExcludeMasterAccount_WhenCalled()
        {
            using (var context = CreateContext())
            {
                var customer = new Customer("Active", "29783013092", "active@example.com", 300);
                var account = new TradingAccount(customer, AccountType.Master);

                context.Customers.Add(customer);
                context.TradingAccounts.Add(account);
                await context.SaveChangesAsync();

                var repo = new CustomerRepository(context);
                var result = await repo.GetActiveCustomers();

                result.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task GetCustomerWithPortfolioAsync_ShouldReturnIncludingCustodies_WhenExists()
        {
            using (var context = CreateContext())
            {
                var customer = new Customer("Test", "29783013092", "test@example.com", 300);
                customer.GetType().GetProperty("Id")?.SetValue(customer, 1);

                var account = new TradingAccount(customer, AccountType.SubAccount);
                account.GetType().GetProperty("Id")?.SetValue(account, 1);
                account.GetType().GetProperty("CustomerId")?.SetValue(account, 1);

                account.AddCustody("PETR4", 10, 20);

                context.Customers.Add(customer);
                context.TradingAccounts.Add(account);
                await context.SaveChangesAsync();

                var repo = new CustomerRepository(context);
                var result = await repo.GetCustomerWithPortfolioAsync(1);

                result.Should().NotBeNull();
                result.TradingAccount.Should().NotBeNull();
                result.TradingAccount.Custodies.Should().NotBeEmpty();
            }
        }

        [Fact]
        public async Task Update_ShouldPersistChanges_WhenEntityIsUpdated()
        {
            var dbName = Guid.NewGuid().ToString();
            int customerId;

            using (var context = CreateContext(dbName))
            {
                var customer = new Customer("Test", "29783013092", "test@example.com", 300);
                context.Customers.Add(customer);
                await context.SaveChangesAsync();
                customerId = customer.Id;

                var repo = new CustomerRepository(context);
                customer.UpdateContribution(500);
                repo.Update(customer);
                await repo.SaveChangesAsync();
            }

            using (var verifyContext = CreateContext(dbName))
            {
                var verifyRepo = new CustomerRepository(verifyContext);
                var result = await verifyRepo.GetByIdAsync(customerId);
                result.MonthlyContribution.Should().Be(500);
            }
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity_WhenCalled()
        {
            var dbName = Guid.NewGuid().ToString();
            int customerId;

            using (var context = CreateContext(dbName))
            {
                var customer = new Customer("Test", "29783013092", "test@example.com", 300);
                context.Customers.Add(customer);
                await context.SaveChangesAsync();
                customerId = customer.Id;

                var repo = new CustomerRepository(context);
                repo.Delete(customer);
                await repo.SaveChangesAsync();
            }

            using (var verifyContext = CreateContext(dbName))
            {
                var verifyRepo = new CustomerRepository(verifyContext);
                var result = await verifyRepo.GetByIdAsync(customerId);
                result.Should().BeNull();
            }
        }
    }
}
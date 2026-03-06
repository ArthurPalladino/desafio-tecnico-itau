using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Data;
using System;
using System.Threading.Tasks;

namespace Tests.Repositories.Tests
{
    public class ContributionHistoryRepositoryTests
    {

        private ItauTopFiveDbContext CreateContext(string databaseName = null)
        {
            var options = new DbContextOptionsBuilder<ItauTopFiveDbContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;

            return new ItauTopFiveDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistHistory()
        {
            var dbName = "Db_AddAsync_Persist";

            using (var context = CreateContext(dbName))
            {
                var customer = new Customer("Test", "12345678909", "test@example.com", 300);
                context.Customers.Add(customer);
                await context.SaveChangesAsync();

                var history = new ContributionHistory(customer, 300, 500);
                var repo = new ContributionHistoryRepository(context);

                await repo.AddAsync(history);
                await repo.SaveChangesAsync();
            }

            using (var verifyContext = CreateContext(dbName))
            {
                var verifyRepo = new ContributionHistoryRepository(verifyContext);
                var result = await verifyRepo.GetAllAsync();

                result.Should().ContainSingle();
                result.ToArray()[0].NewValue.Should().Be(500);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnHistory_WhenExists()
        {
            using (var context = CreateContext())
            {
                var customer = new Customer("Test", "12345678909", "test@example.com", 300);
                var history = new ContributionHistory(customer, 300, 500);

                context.Customers.Add(customer);
                context.ContributionHistory.Add(history);
                await context.SaveChangesAsync();

                var repo = new ContributionHistoryRepository(context);
                var result = await repo.GetByIdAsync(history.Id);

                result.Should().NotBeNull();
                result.NewValue.Should().Be(500);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            using (var context = CreateContext())
            {
                var repo = new ContributionHistoryRepository(context);
                var result = await repo.GetByIdAsync(999);

                result.Should().BeNull();
            }
        }
    }
}

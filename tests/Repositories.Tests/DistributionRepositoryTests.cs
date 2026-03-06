using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Data;
using System;
using System.Threading.Tasks;

namespace Tests.Repositories.Tests
{
    public class DistributionRepositoryTests
    {
        private ItauTopFiveDbContext CreateContext(string databaseName = null)
        {
            var options = new DbContextOptionsBuilder<ItauTopFiveDbContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;
            return new ItauTopFiveDbContext(options);
        }

        [Fact]
        public async Task GetLatestDistributionByTickerAsync_ShouldReturnMostRecent_WhenExists()
        {
            using (var context = CreateContext())
            {
                var purchaseOrder = new PurchaseOrder(1, "PETR4", MarketType.StandardLot, 100, 10);
                context.PurchaseOrders.Add(purchaseOrder);
                await context.SaveChangesAsync();

                var distribution1 = new Distribution(purchaseOrder, 1, "PETR4", 50, 10, DateTime.Now.AddDays(-1));
                var distribution2 = new Distribution(purchaseOrder, 1, "PETR4", 50, 10, DateTime.Now);

                context.Distributions.AddRange(distribution1, distribution2);
                await context.SaveChangesAsync();

                var repo = new DistributionRepository(context);
                var result = await repo.GetLatestDistributionByTickerAsync("PETR4");

                result.Should().NotBeNull();
                result.DistributedAt.Should().BeCloseTo(distribution2.DistributedAt, TimeSpan.FromSeconds(1));
            }
        }

        [Fact]
        public async Task GetLatestDistributionByTickerAsync_ShouldReturnNull_WhenNotFound()
        {
            using (var context = CreateContext())
            {
                var repo = new DistributionRepository(context);
                var result = await repo.GetLatestDistributionByTickerAsync("INVALID");

                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task AddAsync_ShouldPersistDistribution()
        {
            var dbName = Guid.NewGuid().ToString();

            using (var context = CreateContext(dbName))
            {
                var purchaseOrder = new PurchaseOrder(1, "PETR4", MarketType.StandardLot, 100, 10);
                context.PurchaseOrders.Add(purchaseOrder);
                await context.SaveChangesAsync();

                var distribution = new Distribution(purchaseOrder, 1, "PETR4", 50, 10, DateTime.Now);

                var repo = new DistributionRepository(context);
                await repo.AddAsync(distribution);
                await repo.SaveChangesAsync();
            }

            using (var verifyContext = CreateContext(dbName))
            {
                var verifyRepo = new DistributionRepository(verifyContext);
                var result = await verifyRepo.GetAllAsync();
                result.Should().ContainSingle(d => d.Symbol == "PETR4");
            }
        }
    }
}
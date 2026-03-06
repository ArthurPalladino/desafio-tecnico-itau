using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Data;
using System;
using System.Threading.Tasks;

namespace Tests.Repositories.Tests
{
    public class RebalancingRepositoryTests
    {
        private ItauTopFiveDbContext CreateContext(string databaseName = null)
        {
            var options = new DbContextOptionsBuilder<ItauTopFiveDbContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;
            return new ItauTopFiveDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistRebalancing()
        {
            var dbName = Guid.NewGuid().ToString();

            using (var context = CreateContext(dbName))
            {
                var rebalancing = new Rebalancing(1, "PETR4", "VALE3", 50, RebalancingType.OutofBalance);

                var repo = new RebalancingRepository(context);
                await repo.AddAsync(rebalancing);
                await repo.SaveChangesAsync();
            }

            using (var verifyContext = CreateContext(dbName))
            {
                var verifyRepo = new RebalancingRepository(verifyContext);
                var result = await verifyRepo.GetAllAsync();
                result.Should().ContainSingle(r => r.BuyTicker == "VALE3");
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnRebalancing_WhenExists()
        {
            using (var context = CreateContext())
            {
                var rebalancing = new Rebalancing(1, "PETR4", "VALE3", 50, RebalancingType.OutofBalance);
                context.Rebalancings.Add(rebalancing);
                await context.SaveChangesAsync();

                var repo = new RebalancingRepository(context);
                var result = await repo.GetByIdAsync(rebalancing.Id);

                result.Should().NotBeNull();
                result.Quantity.Should().Be(50);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            using (var context = CreateContext())
            {
                var repo = new RebalancingRepository(context);
                var result = await repo.GetByIdAsync(999);

                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task Delete_ShouldRemoveRebalancing()
        {
            var dbName = Guid.NewGuid().ToString();
            int rebalancingId;

            using (var context = CreateContext(dbName))
            {
                var rebalancing = new Rebalancing(1, "PETR4", "VALE3", 50, RebalancingType.OutofBalance);
                context.Rebalancings.Add(rebalancing);
                await context.SaveChangesAsync();
                rebalancingId = rebalancing.Id;

                var repo = new RebalancingRepository(context);
                repo.Delete(rebalancing);
                await repo.SaveChangesAsync();
            }

            using (var verifyContext = CreateContext(dbName))
            {
                var verifyRepo = new RebalancingRepository(verifyContext);
                var result = await verifyRepo.GetByIdAsync(rebalancingId);
                result.Should().BeNull();
            }
        }
    }
}
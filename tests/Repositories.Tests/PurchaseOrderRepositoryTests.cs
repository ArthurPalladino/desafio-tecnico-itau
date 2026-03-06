using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Data;
using System;
using System.Threading.Tasks;

namespace Tests.Repositories.Tests
{
    public class PurchaseOrderRepositoryTests
    {
        private ItauTopFiveDbContext CreateContext(string databaseName = null)
        {
            var options = new DbContextOptionsBuilder<ItauTopFiveDbContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;
            return new ItauTopFiveDbContext(options);
        }

        [Fact]
        public async Task GetLastOrderAsync_ShouldReturnMostRecent_WhenExists()
        {
            using (var context = CreateContext())
            {
                var order1 = new PurchaseOrder(1, "PETR4", MarketType.StandardLot, 100, 10);
                order1.GetType().GetProperty("ExecutionDate")?.SetValue(order1, DateTime.Now.AddDays(-1));

                var order2 = new PurchaseOrder(1, "PETR4", MarketType.Fractional, 50, 10);
                order2.GetType().GetProperty("ExecutionDate")?.SetValue(order2, DateTime.Now);

                context.PurchaseOrders.AddRange(order1, order2);
                await context.SaveChangesAsync();

                var repo = new PurchaseOrderRepository(context);
                var result = await repo.GetLastOrderAsync("PETR4");

                result.Should().NotBeNull();
                result.Quantity.Should().Be(50);
            }
        }

        [Fact]
        public async Task GetLastOrderAsync_ShouldReturnNull_WhenNotFound()
        {
            using (var context = CreateContext())
            {
                var repo = new PurchaseOrderRepository(context);
                var result = await repo.GetLastOrderAsync("INVALID");

                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetTotalSalesValueInMonthAsync_ShouldSumNegativeQuantityOrders()
        {
            using (var context = CreateContext())
            {
                var date = new DateTime(2026, 3, 10);

                var order1 = new PurchaseOrder(1, "PETR4", MarketType.StandardLot, -50, 10);
                order1.GetType().GetProperty("ExecutionDate")?.SetValue(order1, date);

                var order2 = new PurchaseOrder(1, "PETR4", MarketType.StandardLot, -30, 10);
                order2.GetType().GetProperty("ExecutionDate")?.SetValue(order2, date);

                var order3 = new PurchaseOrder(1, "PETR4", MarketType.StandardLot, 100, 10);
                order3.GetType().GetProperty("ExecutionDate")?.SetValue(order3, date);

                context.PurchaseOrders.AddRange(order1, order2, order3);
                await context.SaveChangesAsync();

                var repo = new PurchaseOrderRepository(context);
                var result = await repo.GetTotalSalesValueInMonthAsync(1, 2026, 3);

                result.Should().Be(800m);
            }
        }

        [Fact]
        public async Task GetTotalSalesValueInMonthAsync_ShouldReturnZero_WhenNoSales()
        {
            using (var context = CreateContext())
            {
                var order = new PurchaseOrder(1, "PETR4", MarketType.StandardLot, 100, 10);
                context.PurchaseOrders.Add(order);
                await context.SaveChangesAsync();

                var repo = new PurchaseOrderRepository(context);
                var result = await repo.GetTotalSalesValueInMonthAsync(1, 2026, 3);

                result.Should().Be(0m);
            }
        }

        [Fact]
        public async Task AddAsync_ShouldPersistOrder()
        {
            var dbName = Guid.NewGuid().ToString();

            using (var context = CreateContext(dbName))
            {
                var order = new PurchaseOrder(1, "PETR4", MarketType.StandardLot, 100, 10);

                var repo = new PurchaseOrderRepository(context);
                await repo.AddAsync(order);
                await repo.SaveChangesAsync();
            }

            using (var verifyContext = CreateContext(dbName))
            {
                var verifyRepo = new PurchaseOrderRepository(verifyContext);
                var result = await verifyRepo.GetAllAsync();
                result.Should().ContainSingle(o => o.Symbol == "PETR4");
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnOrder_WhenExists()
        {
            using (var context = CreateContext())
            {
                var order = new PurchaseOrder(1, "PETR4", MarketType.StandardLot, 100, 10);
                context.PurchaseOrders.Add(order);
                await context.SaveChangesAsync();

                var repo = new PurchaseOrderRepository(context);
                var result = await repo.GetByIdAsync(order.Id);

                result.Should().NotBeNull();
                result.Quantity.Should().Be(100);
            }
        }
    }
}
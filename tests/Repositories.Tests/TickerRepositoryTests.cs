using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Repositories.Tests
{
    public class TickerRepositoryTests
    {
        private ItauTopFiveDbContext CreateContext(string databaseName = null)
        {
            var options = new DbContextOptionsBuilder<ItauTopFiveDbContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;
            return new ItauTopFiveDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistTicker()
        {
            var dbName = Guid.NewGuid().ToString();

            using (var context = CreateContext(dbName))
            {
                var ticker = new Ticker("PETR4", 10, DateTime.Now);
                var repo = new TickerRepository(context);

                await repo.AddAsync(ticker);
                await repo.SaveChangesAsync();
            }

            using (var verifyContext = CreateContext(dbName))
            {
                var verifyRepo = new TickerRepository(verifyContext);
                var result = await verifyRepo.GetAllAsync();
                result.Should().ContainSingle(t => t.Symbol == "PETR4");
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnTicker_WhenExists()
        {
            using (var context = CreateContext())
            {
                var ticker = new Ticker("PETR4", 10, DateTime.Now);
                context.Tickers.Add(ticker);
                await context.SaveChangesAsync();

                var repo = new TickerRepository(context);
                var result = await repo.GetByIdAsync(ticker.Id);

                result.Should().NotBeNull();
                result.Symbol.Should().Be("PETR4");
            }
        }

        [Fact]
        public async Task GetUniqueSymbols_ShouldReturnDistinctSymbols()
        {
            using (var context = CreateContext())
            {
                var ticker1 = new Ticker("PETR4", 10, DateTime.Now);
                var ticker2 = new Ticker("PETR4", 11, DateTime.Now.AddDays(-1));
                var ticker3 = new Ticker("VALE3", 20, DateTime.Now);

                context.Tickers.AddRange(ticker1, ticker2, ticker3);
                await context.SaveChangesAsync();

                var repo = new TickerRepository(context);
                var result = await repo.GetUniqueSymbols();

                result.Should().HaveCount(2);
                result.Should().Contain("PETR4");
                result.Should().Contain("VALE3");
            }
        }

        [Fact]
        public async Task GetBySymbolAsync_ShouldReturnLatestPrice_WhenSymbolExists()
        {
            using (var context = CreateContext())
            {
                var oldDate = DateTime.Now.AddDays(-1);
                var newDate = DateTime.Now;

                var ticker1 = new Ticker("PETR4", 10m, oldDate);
                var ticker2 = new Ticker("PETR4", 15m, newDate);

                context.Tickers.AddRange(ticker1, ticker2);
                await context.SaveChangesAsync();

                var repo = new TickerRepository(context);
                var result = await repo.GetBySymbolAsync("PETR4", newDate);

                result.Should().NotBeNull();
                result.CurrentPrice.Should().Be(15m);
            }
        }

        [Fact]
        public async Task GetBySymbolAsync_ShouldReturnNull_WhenSymbolNotFound()
        {
            using (var context = CreateContext())
            {
                var repo = new TickerRepository(context);
                var result = await repo.GetBySymbolAsync("INVALID", DateTime.Now);

                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetLatestByTickerAsync_ShouldReturnMostRecentPrice()
        {
            using (var context = CreateContext())
            {
                var oldDate = DateTime.Now.AddDays(-1);
                var newDate = DateTime.Now;

                var ticker1 = new Ticker("PETR4", 10m, oldDate);
                var ticker2 = new Ticker("PETR4", 15m, newDate);

                context.Tickers.AddRange(ticker1, ticker2);
                await context.SaveChangesAsync();

                var repo = new TickerRepository(context);
                var result = await repo.GetLatestByTickerAsync("PETR4");

                result.Should().NotBeNull();
                result.CurrentPrice.Should().Be(15m);
            }
        }

        [Fact]
        public async Task GetLatestByTickerAsync_ShouldReturnNull_WhenNotFound()
        {
            using (var context = CreateContext())
            {
                var repo = new TickerRepository(context);
                var result = await repo.GetLatestByTickerAsync("INVALID");

                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task AddRangeAsync_ShouldPersistMultipleTickers()
        {
            var dbName = Guid.NewGuid().ToString();

            using (var context = CreateContext(dbName))
            {
                var tickers = new List<Ticker>
                {
                    new Ticker("PETR4", 10, DateTime.Now),
                    new Ticker("VALE3", 20, DateTime.Now),
                    new Ticker("ITUB4", 30, DateTime.Now)
                };

                var repo = new TickerRepository(context);
                await repo.AddRangeAsync(tickers);
                await repo.SaveChangesAsync();
            }

            using (var verifyContext = CreateContext(dbName))
            {
                var verifyRepo = new TickerRepository(verifyContext);
                var result = await verifyRepo.GetAllAsync();
                result.Should().HaveCount(3);
            }
        }
    }
}
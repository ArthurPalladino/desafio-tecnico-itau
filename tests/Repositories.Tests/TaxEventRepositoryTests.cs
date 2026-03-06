using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Data;
using System;
using System.Threading.Tasks;

namespace Tests.Repositories.Tests
{
    public class TaxEventRepositoryTests
    {
        private ItauTopFiveDbContext CreateContext(string databaseName = null)
        {
            var options = new DbContextOptionsBuilder<ItauTopFiveDbContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;
            return new ItauTopFiveDbContext(options);
        }

        [Fact]
        public async Task GetTotalBaseValueInMonthAsync_ShouldSumValuesInMonth()
        {
            using (var context = CreateContext())
            {
                var date1 = new DateTime(2026, 3, 10);
                var date2 = new DateTime(2026, 3, 15);
                var dateOutOfMonth = new DateTime(2026, 4, 10);

                var event1 = new TaxEvent(1, 1000, 50, TaxType.DedoDuro);
                event1.GetType().GetProperty("EventDate")?.SetValue(event1, date1);

                var event2 = new TaxEvent(1, 2000, 100, TaxType.DedoDuro);
                event2.GetType().GetProperty("EventDate")?.SetValue(event2, date2);

                var event3 = new TaxEvent(1, 3000, 150, TaxType.DedoDuro);
                event3.GetType().GetProperty("EventDate")?.SetValue(event3, dateOutOfMonth);

                context.TaxEvents.AddRange(event1, event2, event3);
                await context.SaveChangesAsync();

                var repo = new TaxEventRepository(context);
                var result = await repo.GetTotalBaseValueInMonthAsync(1, TaxType.DedoDuro, 2026, 3);

                result.Should().Be(3000m);
            }
        }

        [Fact]
        public async Task GetTotalBaseValueInMonthAsync_ShouldReturnZero_WhenNoEvents()
        {
            using (var context = CreateContext())
            {
                var repo = new TaxEventRepository(context);
                var result = await repo.GetTotalBaseValueInMonthAsync(1, TaxType.DedoDuro, 2026, 3);

                result.Should().Be(0m);
            }
        }

        [Fact]
        public async Task GetTotalBaseValueInMonthAsync_ShouldFilterByType()
        {
            using (var context = CreateContext())
            {
                var date = new DateTime(2026, 3, 10);

                var event1 = new TaxEvent(1, 1000, 50, TaxType.DedoDuro);
                event1.GetType().GetProperty("EventDate")?.SetValue(event1, date);

                var event2 = new TaxEvent(1, 2000, 100, TaxType.Venda);
                event2.GetType().GetProperty("EventDate")?.SetValue(event2, date);

                context.TaxEvents.AddRange(event1, event2);
                await context.SaveChangesAsync();

                var repo = new TaxEventRepository(context);
                var result = await repo.GetTotalBaseValueInMonthAsync(1, TaxType.DedoDuro, 2026, 3);

                result.Should().Be(1000m);
            }
        }

        [Fact]
        public async Task GetTotalIRValueInMonthAsync_ShouldSumTaxValues()
        {
            using (var context = CreateContext())
            {
                var date1 = new DateTime(2026, 3, 10);
                var date2 = new DateTime(2026, 3, 15);

                var event1 = new TaxEvent(1, 1000, 50, TaxType.DedoDuro);
                event1.GetType().GetProperty("EventDate")?.SetValue(event1, date1);

                var event2 = new TaxEvent(1, 2000, 100, TaxType.DedoDuro);
                event2.GetType().GetProperty("EventDate")?.SetValue(event2, date2);

                context.TaxEvents.AddRange(event1, event2);
                await context.SaveChangesAsync();

                var repo = new TaxEventRepository(context);
                var result = await repo.GetTotalIRValueInMonthAsync(1, TaxType.DedoDuro, 2026, 3);

                result.Should().Be(150m);
            }
        }

        [Fact]
        public async Task GetTotalIRValueInMonthAsync_ShouldReturnZero_WhenNoEvents()
        {
            using (var context = CreateContext())
            {
                var repo = new TaxEventRepository(context);
                var result = await repo.GetTotalIRValueInMonthAsync(1, TaxType.Venda, 2026, 3);

                result.Should().Be(0m);
            }
        }

        [Fact]
        public async Task AddAsync_ShouldPersistEvent()
        {
            var dbName = Guid.NewGuid().ToString();

            using (var context = CreateContext(dbName))
            {
                var taxEvent = new TaxEvent(1, 1000, 50, TaxType.DedoDuro);

                var repo = new TaxEventRepository(context);
                await repo.AddAsync(taxEvent);
                await repo.SaveChangesAsync();
            }

            using (var verifyContext = CreateContext(dbName))
            {
                var verifyRepo = new TaxEventRepository(verifyContext);
                var result = await verifyRepo.GetAllAsync();
                result.Should().ContainSingle();
            }
        }
    }
}
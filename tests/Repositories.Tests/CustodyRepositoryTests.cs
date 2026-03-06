using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Data;
using System;
using System.Threading.Tasks;

namespace Tests.Repositories.Tests
{
    public class CustodyRepositoryTests
    {
        private ItauTopFiveDbContext CreateContext(string databaseName = null)
        {
            var options = new DbContextOptionsBuilder<ItauTopFiveDbContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;
            return new ItauTopFiveDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistCustody()
        {
            var dbName = Guid.NewGuid().ToString();

            using (var context = CreateContext(dbName))
            {
                var custody = new Custody(1, 1, "PETR4", 100, 10);
                var repo = new CustodyRepository(context);

                await repo.AddAsync(custody);
                await repo.SaveChangesAsync();
            }

            using (var verifyContext = CreateContext(dbName))
            {
                var verifyRepo = new CustodyRepository(verifyContext);
                var result = await verifyRepo.GetAllAsync();
                result.Should().ContainSingle(c => c.Symbol == "PETR4");
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCustody_WhenExists()
        {
            using (var context = CreateContext())
            {
                var custody = new Custody(1, 1, "PETR4", 100, 10);
                context.Custodies.Add(custody);
                await context.SaveChangesAsync();

                var repo = new CustodyRepository(context);
                var result = await repo.GetByIdAsync(custody.Id);

                result.Should().NotBeNull();
                result.Quantity.Should().Be(100);
            }
        }

        [Fact]
        public async Task Update_ShouldPersistChanges()
        {
            var dbName = Guid.NewGuid().ToString();
            int custodyId;

            using (var context = CreateContext(dbName))
            {
                var custody = new Custody(1, 1, "PETR4", 100, 10);
                context.Custodies.Add(custody);
                await context.SaveChangesAsync();
                custodyId = custody.Id;

                var repo = new CustodyRepository(context);
                custody.AddQuantity(50, 15);
                repo.Update(custody);
                await repo.SaveChangesAsync();
            }

            using (var verifyContext = CreateContext(dbName))
            {
                var verifyRepo = new CustodyRepository(verifyContext);
                var result = await verifyRepo.GetByIdAsync(custodyId);
                result.Quantity.Should().Be(150);
            }
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            var dbName = Guid.NewGuid().ToString();
            int custodyId;

            using (var context = CreateContext(dbName))
            {
                var custody = new Custody(1, 1, "PETR4", 100, 10);
                context.Custodies.Add(custody);
                await context.SaveChangesAsync();
                custodyId = custody.Id;

                var repo = new CustodyRepository(context);
                repo.Delete(custody);
                await repo.SaveChangesAsync();
            }

            using (var verifyContext = CreateContext(dbName))
            {
                var verifyRepo = new CustodyRepository(verifyContext);
                var result = await verifyRepo.GetByIdAsync(custodyId);
                result.Should().BeNull();
            }
        }
    }
}
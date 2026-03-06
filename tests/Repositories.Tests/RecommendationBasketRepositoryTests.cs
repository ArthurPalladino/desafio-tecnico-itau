using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Repositories.Tests
{
    public class RecommendationBasketRepositoryTests
    {
        private ItauTopFiveDbContext CreateContext(string databaseName = null)
        {
            var options = new DbContextOptionsBuilder<ItauTopFiveDbContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;
            return new ItauTopFiveDbContext(options);
        }

        [Fact]
        public async Task GetActiveBasketWithItensAsync_ShouldReturnActiveBasket_WhenExists()
        {
            using (var context = CreateContext())
            {
                var itens = new List<BasketItem>
                {
                    new BasketItem("PETR4", 20),
                    new BasketItem("VALE3", 20),
                    new BasketItem("ITUB4", 20),
                    new BasketItem("BBDC4", 20),
                    new BasketItem("ABEV3", 20)
                };
                var basket = new RecommendationBasket("Active Basket", itens);

                context.RecommendationBaskets.Add(basket);
                await context.SaveChangesAsync();

                var repo = new RecommendationBasketRepository(context);
                var result = await repo.GetActiveBasketWithItensAsync();

                result.Should().NotBeNull();
                result.IsActive.Should().BeTrue();
                result.Itens.Should().HaveCount(5);
            }
        }

        [Fact]
        public async Task GetActiveBasketWithItensAsync_ShouldReturnNull_WhenNoActiveBasket()
        {
            using (var context = CreateContext())
            {
                var itens = new List<BasketItem>
                {
                    new BasketItem("PETR4", 20),
                    new BasketItem("VALE3", 20),
                    new BasketItem("ITUB4", 20),
                    new BasketItem("BBDC4", 20),
                    new BasketItem("ABEV3", 20)
                };
                var basket = new RecommendationBasket("Inactive Basket", itens);
                basket.Deactivate();

                context.RecommendationBaskets.Add(basket);
                await context.SaveChangesAsync();

                var repo = new RecommendationBasketRepository(context);
                var result = await repo.GetActiveBasketWithItensAsync();

                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetActiveBasketWithItensAsync_ShouldReturnFirstActiveOnly_WhenMultipleExist()
        {
            using (var context = CreateContext())
            {
                var itens1 = new List<BasketItem>
                {
                    new BasketItem("PETR4", 20),
                    new BasketItem("VALE3", 20),
                    new BasketItem("ITUB4", 20),
                    new BasketItem("BBDC4", 20),
                    new BasketItem("ABEV3", 20)
                };
                var basket1 = new RecommendationBasket("Basket1", itens1);

                var itens2 = new List<BasketItem>
                {
                    new BasketItem("RAIL3", 20),
                    new BasketItem("PSSA3", 20),
                    new BasketItem("WEGE3", 20),
                    new BasketItem("CSAN3", 20),
                    new BasketItem("SUZB3", 20)
                };
                var basket2 = new RecommendationBasket("Basket2", itens2);
                basket2.Deactivate();

                context.RecommendationBaskets.AddRange(basket1, basket2);
                await context.SaveChangesAsync();

                var repo = new RecommendationBasketRepository(context);
                var result = await repo.GetActiveBasketWithItensAsync();

                result.Should().NotBeNull();
                result.Name.Should().Be("Basket1");
            }
        }

        [Fact]
        public async Task GetHistoryAsync_ShouldReturnAllBaskets_WhenCalled()
        {
            using (var context = CreateContext())
            {
                var itens1 = new List<BasketItem>
                {
                    new BasketItem("PETR4", 20),
                    new BasketItem("VALE3", 20),
                    new BasketItem("ITUB4", 20),
                    new BasketItem("BBDC4", 20),
                    new BasketItem("ABEV3", 20)
                };
                var basket1 = new RecommendationBasket("Basket1", itens1);

                var itens2 = new List<BasketItem>
                {
                    new BasketItem("RAIL3", 20),
                    new BasketItem("PSSA3", 20),
                    new BasketItem("WEGE3", 20),
                    new BasketItem("CSAN3", 20),
                    new BasketItem("SUZB3", 20)
                };
                var basket2 = new RecommendationBasket("Basket2", itens2);
                basket2.Deactivate();

                context.RecommendationBaskets.AddRange(basket1, basket2);
                await context.SaveChangesAsync();

                var repo = new RecommendationBasketRepository(context);
                var result = await repo.GetHistoryAsync();

                result.Should().HaveCount(2);
            }
        }

        [Fact]
        public async Task GetHistoryAsync_ShouldReturnOrderedByDateDescending()
        {
            using (var context = CreateContext())
            {
                var itens1 = new List<BasketItem>
                {
                    new BasketItem("PETR4", 20),
                    new BasketItem("VALE3", 20),
                    new BasketItem("ITUB4", 20),
                    new BasketItem("BBDC4", 20),
                    new BasketItem("ABEV3", 20)
                };
                var basket1 = new RecommendationBasket("OldBasket", itens1);
                basket1.GetType().GetProperty("CreatedAt")?.SetValue(basket1, DateTime.UtcNow.AddDays(-1));

                var itens2 = new List<BasketItem>
                {
                    new BasketItem("RAIL3", 20),
                    new BasketItem("PSSA3", 20),
                    new BasketItem("WEGE3", 20),
                    new BasketItem("CSAN3", 20),
                    new BasketItem("SUZB3", 20)
                };
                var basket2 = new RecommendationBasket("NewBasket", itens2);

                context.RecommendationBaskets.AddRange(basket1, basket2);
                await context.SaveChangesAsync();

                var repo = new RecommendationBasketRepository(context);
                var result = await repo.GetHistoryAsync();

                result[0].Name.Should().Be("NewBasket");
                result[1].Name.Should().Be("OldBasket");
            }
        }

        [Fact]
        public async Task AddAsync_ShouldPersistBasket()
        {
            var dbName = Guid.NewGuid().ToString();

            using (var context = CreateContext(dbName))
            {
                var itens = new List<BasketItem>
                {
                    new BasketItem("PETR4", 20),
                    new BasketItem("VALE3", 20),
                    new BasketItem("ITUB4", 20),
                    new BasketItem("BBDC4", 20),
                    new BasketItem("ABEV3", 20)
                };
                var basket = new RecommendationBasket("TestBasket", itens);

                var repo = new RecommendationBasketRepository(context);
                await repo.AddAsync(basket);
                await repo.SaveChangesAsync();
            }

            using (var verifyContext = CreateContext(dbName))
            {
                var verifyRepo = new RecommendationBasketRepository(verifyContext);
                var result = await verifyRepo.GetAllAsync();
                result.Should().ContainSingle(b => b.Name == "TestBasket");
            }
        }
    }
}
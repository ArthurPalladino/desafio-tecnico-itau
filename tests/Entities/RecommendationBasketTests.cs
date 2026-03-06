using FluentAssertions;
using Xunit;
using System.Collections.Generic;

public class RecommendationBasketTests
{
    [Fact]
    public void Constructor_ShouldCreate_WhenValid()
    {
        var itens = new List<BasketItem>
        {
            new BasketItem("A",20),
            new BasketItem("B",20),
            new BasketItem("C",20),
            new BasketItem("D",20),
            new BasketItem("E",20)
        };
        var basket = new RecommendationBasket("Name", itens);
        basket.IsActive.Should().BeTrue();
        basket.Itens.Should().HaveCount(5);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenCountNotFive()
    {
        var itens = new List<BasketItem> { new BasketItem("A",100) };
        Action act = () => new RecommendationBasket("Name", itens);
        act.Should().Throw<CustomException>().WithMessage("A cesta nao contem exatamente 5 ativos.");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenPercentagesNot100()
    {
        var itens = new List<BasketItem>
        {
            new BasketItem("A",50),
            new BasketItem("B",50),
            new BasketItem("C",1),
            new BasketItem("D",1),
            new BasketItem("E",1)
        };
        Action act = () => new RecommendationBasket("Name", itens);
        act.Should().Throw<CustomException>().WithMessage("A soma dos percentuais deve ser exatamente 100%.");
    }

    [Fact]
    public void Deactivate_ShouldMarkInactive()
    {
        var itens = new List<BasketItem>
        {
            new BasketItem("A",20),
            new BasketItem("B",20),
            new BasketItem("C",20),
            new BasketItem("D",20),
            new BasketItem("E",20)
        };
        var basket = new RecommendationBasket("Name", itens);
        basket.Deactivate();
        basket.IsActive.Should().BeFalse();
        basket.DeactivatedAt.Should().NotBeNull();
    }
}

using FluentAssertions;
using Xunit;
using System;
using System.Collections.Generic;

public class DistributionTests
{
    [Fact]
    public void Constructor_ShouldCreate_WhenValidWithOrder()
    {
        var order = new PurchaseOrder(1, "SYM", MarketType.StandardLot, 1, 10m);
        var dist = new Distribution(order, 2, "SYM", 5, 10m, DateTime.Now);
        dist.ChildAccountId.Should().Be(2);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenOrderNull()
    {
        Action act = () => new Distribution(null!, 2, "SYM", 5, 10m, DateTime.Now);
        act.Should().Throw<CustomException>().WithMessage("A ordem de compra informada é inválida.");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenInvalidFields()
    {
        var order = new PurchaseOrder(1, "SYM", MarketType.StandardLot, 1, 10m);
        Action act1 = () => new Distribution(order, 0, "SYM", 5, 10m, DateTime.Now);
        act1.Should().Throw<CustomException>().WithMessage("A conta filhote informada é inválida.");
        Action act2 = () => new Distribution(order, 2, "", 5, 10m, DateTime.Now);
        act2.Should().Throw<CustomException>().WithMessage("O símbolo do ativo (ticker) é obrigatório.");
        Action act3 = () => new Distribution(order, 2, "SYM", -1, 10m, DateTime.Now);
        act3.Should().Throw<CustomException>().WithMessage("A quantidade não pode ser negativa.");
        Action act4 = () => new Distribution(order, 2, "SYM", 5, -1m, DateTime.Now);
        act4.Should().Throw<CustomException>().WithMessage("O preço não pode ser negativo.");
    }
}

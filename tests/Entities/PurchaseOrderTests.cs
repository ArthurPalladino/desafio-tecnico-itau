using FluentAssertions;
using Xunit;
using System;

public class PurchaseOrderTests
{
    [Fact]
    public void Constructor_ShouldCreate_WhenValid()
    {
        var order = new PurchaseOrder(1, "SYM", MarketType.StandardLot, 10, 5m);
        order.MasterAccountId.Should().Be(1);
        order.Symbol.Should().Be("SYM");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenInvalid()
    {
        Action act1 = () => new PurchaseOrder(0, "SYM", MarketType.StandardLot, 10, 5m);
        act1.Should().Throw<CustomException>().WithMessage("A conta master informada é inválida.");
        Action act2 = () => new PurchaseOrder(1, "", MarketType.StandardLot, 10, 5m);
        act2.Should().Throw<CustomException>().WithMessage("O símbolo do ativo (ticker) é obrigatório.");
        Action act3 = () => new PurchaseOrder(1, "SYM", MarketType.StandardLot, 10, -1m);
        act3.Should().Throw<CustomException>().WithMessage("O preço não pode ser negativo.");
    }
}

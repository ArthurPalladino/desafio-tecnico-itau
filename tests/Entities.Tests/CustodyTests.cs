using System;
using FluentAssertions;
using Xunit;

public class CustodyTests
{
    [Fact]
    public void Constructor_ShouldCreate_WhenDataIsValid()
    {
        var custody = new Custody(1, 2, "SYM", 5, 10m);
        custody.Symbol.Should().Be("SYM");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenInvalidValues()
    {
        Action act1 = () => new Custody(0, 2, "SYM", 5, 10m);
        act1.Should().Throw<CustomException>().WithMessage("O ID do cliente é inválido.");
        Action act2 = () => new Custody(1, 0, "SYM", 5, 10m);
        act2.Should().Throw<CustomException>().WithMessage("O ID da conta gráfica é inválido.");
        Action act3 = () => new Custody(1, 2, "", 5, 10m);
        act3.Should().Throw<CustomException>().WithMessage("O símbolo do ativo (ticker) é obrigatório.");
        Action act4 = () => new Custody(1, 2, "SYM", -1, 10m);
        act4.Should().Throw<CustomException>().WithMessage("A quantidade não pode ser negativa.");
        Action act5 = () => new Custody(1, 2, "SYM", 5, -1m);
        act5.Should().Throw<CustomException>().WithMessage("O preço não pode ser negativo.");
    }

    [Fact]
    public void AddQuantity_ShouldUpdateAverage_WhenCalled()
    {
        var custody = new Custody(1, 2, "SYM", 5, 10m);
        custody.AddQuantity(5, 20m);
        custody.Quantity.Should().Be(10);
        custody.AveragePrice.Should().Be((5*10m + 5*20m)/10);
    }

    [Fact]
    public void AddQuantity_ShouldThrow_WhenInvalid()
    {
        var custody = new Custody(1, 2, "SYM", 5, 10m);
        Action act1 = () => custody.AddQuantity(0, 10m);
        act1.Should().Throw<CustomException>().WithMessage("A quantidade não pode ser negativa.");
        Action act2 = () => custody.AddQuantity(1, -1m);
        act2.Should().Throw<CustomException>().WithMessage("O preço não pode ser negativo.");
    }

    [Fact]
    public void RemoveQuantity_ShouldSubtract_WhenValid()
    {
        var custody = new Custody(1, 2, "SYM", 5, 10m);
        custody.RemoveQuantity(3);
        custody.Quantity.Should().Be(2);
    }

    [Fact]
    public void RemoveQuantity_ShouldThrow_WhenInvalid()
    {
        var custody = new Custody(1, 2, "SYM", 5, 10m);
        Action act1 = () => custody.RemoveQuantity(0);
        act1.Should().Throw<CustomException>().WithMessage("A quantidade não pode ser negativa.");
        Action act2 = () => custody.RemoveQuantity(6);
        act2.Should().Throw<CustomException>().WithMessage("Quantidade insuficiente em custódia.");
    }
}

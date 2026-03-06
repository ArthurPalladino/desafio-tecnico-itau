using System;
using FluentAssertions;
using Xunit;

public class TickerTests
{
    [Fact]
    public void Constructor_ShouldCreate_WhenDataIsValid()
    {
        var date = DateTime.Now;
        var ticker = new Ticker("SYM", 10m, date);
        ticker.Symbol.Should().Be("SYM");
        ticker.CurrentPrice.Should().Be(10m);
        ticker.PriceDate.Should().Be(date);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenSymbolEmpty()
    {
        Action act = () => new Ticker("", 10m, DateTime.Now);
        act.Should().Throw<CustomException>().WithMessage("O símbolo do ativo (ticker) é obrigatório.");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenPriceNegative()
    {
        Action act = () => new Ticker("SYM", -1m, DateTime.Now);
        act.Should().Throw<CustomException>().WithMessage("O preço não pode ser negativo.");
    }
}

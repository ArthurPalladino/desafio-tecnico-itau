using FluentAssertions;
using Xunit;

public class BasketItemTests
{
    [Fact]
    public void Constructor_ShouldCreate_WhenValid()
    {
        var item = new BasketItem("SYM", 50);
        item.Symbol.Should().Be("SYM");
        item.Percentage.Should().Be(50);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenSymbolEmpty()
    {
        Action act = () => new BasketItem("", 50);
        act.Should().Throw<CustomException>().WithMessage("O símbolo do ativo (ticker) é obrigatório.");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenPercentageInvalid()
    {
        Action act1 = () => new BasketItem("SYM", 0);
        act1.Should().Throw<CustomException>().WithMessage("O percentual deve ser maior que 0 e menor ou igual a 100.");
        Action act2 = () => new BasketItem("SYM", 101);
        act2.Should().Throw<CustomException>().WithMessage("O percentual deve ser maior que 0 e menor ou igual a 100.");
    }
}

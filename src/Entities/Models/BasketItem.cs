using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class BasketItem
{

    public int Id { get; private set; }


    public int RecommendationBasketId { get; private set; }


    public string Symbol { get; private set; } = string.Empty;


    public decimal Percentage { get; private set; }

    // EF ctor
    protected BasketItem() { }

    public BasketItem(string symbol, decimal percentage)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("O símbolo do ativo (ticker) é obrigatório.", nameof(symbol));

        if (percentage <= 0 || percentage > 100)
            throw new ArgumentOutOfRangeException(nameof(percentage), "O percentual deve ser maior que 0 e menor ou igual a 100.");

        Symbol = symbol.Trim().ToUpperInvariant();
        Percentage = percentage;
    }
}
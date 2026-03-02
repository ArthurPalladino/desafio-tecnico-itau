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
            throw new CustomException("TICKER_OBRIGATORIO");

        if (percentage <= 0 || percentage > 100)
            throw new CustomException("PERCENTUAL_INVALIDO");
            
        Symbol = symbol.Trim().ToUpperInvariant();
        Percentage = percentage;
    }
}
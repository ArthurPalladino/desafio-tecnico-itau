using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Ticker
{

    public int Id { get; private set; }

    public string Symbol { get; private set; } = string.Empty;

    public decimal CurrentPrice { get; private set; }


    public DateTime PriceDate {get; private set;}
    // EF only
    protected Ticker() { }

    public Ticker(string symbol, decimal currentPrice, DateTime date)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new CustomException("TICKER_OBRIGATORIO");

        if (currentPrice < 0)
            throw new CustomException("PRECO_NEGATIVO");
        Symbol = symbol.Trim().ToUpperInvariant();
        CurrentPrice = currentPrice;
        PriceDate = date;
    }

}
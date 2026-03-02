using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RecommendationBasket
{
    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? DeactivatedAt { get; private set; }

    public List<BasketItem> Itens { get; private set; } = new();

    // EF
    protected RecommendationBasket() { } 

    public RecommendationBasket(string name, List<BasketItem> itens)
    {
        if (itens.Count != 5)
            throw new CustomException("QUANTIDADE_ATIVOS_INVALIDA");

        if (itens.Sum(i => i.Percentage) != 100)
            throw new CustomException("PERCENTUAIS_INVALIDOS");
        Name = name;
        Itens = itens;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        DeactivatedAt = DateTime.UtcNow;
    }
}
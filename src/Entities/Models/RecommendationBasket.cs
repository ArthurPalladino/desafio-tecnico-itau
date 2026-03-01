using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RecommendationBasket
{
    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? DeactivatedAt { get; private set; }

    public List<BasketItem> Items { get; private set; } = new();

    // EF
    protected RecommendationBasket() { } 

    public RecommendationBasket(string name, List<BasketItem> items)
    {
        if (items.Count != 5)
            throw new ArgumentException($"A cesta deve conter exatamente 5 ativos. Quantidade informada: {items.Count}.");

        if (items.Sum(i => i.Percentage) != 100)
            throw new ArgumentException($"A soma dos percentuais deve ser exatamente 100%. Soma atual: {items.Sum(i => i.Percentage)}%.");

        Name = name;
        Items = items;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        DeactivatedAt = DateTime.UtcNow;
    }
}
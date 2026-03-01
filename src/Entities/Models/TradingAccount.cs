using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TradingAccount
{
    public int Id { get; private set; }

    public int CustomerId { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public AccountType Type { get; private set; }
    
    public Customer Customer { get; private set; } = null!;
    public List<Custody> Custodies { get; private set; } = new();

    // EF ctor
    protected TradingAccount() { }

    public TradingAccount(int customerId, string description, AccountType type)
    {
        if (customerId <= 0)
            throw new ArgumentException("O ID do cliente é inválido.", nameof(customerId));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("A descrição da conta é obrigatória.", nameof(description));

        CustomerId = customerId;
        Description = description;
        Type = type;
    }

    public void UpdateDescription(string newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription))
            throw new ArgumentException("A descrição não pode estar vazia.", nameof(newDescription));
            
        Description = newDescription;
    }
}
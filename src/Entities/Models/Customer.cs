using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Customer
{

    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;


    public string Cpf { get; private set; } = string.Empty;


    public string Email { get; private set; } = string.Empty;

    public bool IsActive { get; private set; } = true;
    
    public decimal MonthlyContribution { get; private set; }

    public DateTime SubscriptionDate { get; private set; }
    
    public TradingAccount TradingAccount { get; private set; } = null!;
    public List<Custody> Custodies { get; private set; } = new();

    // EF
    protected Customer() { }

    public Customer(string name, string cpf, string email, decimal contribution)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome é obrigatório.", nameof(name));
        if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11)
            throw new ArgumentException("CPF inválido. O campo deve conter exatamente 11 dígitos numéricos.", nameof(cpf));
        if (contribution < 100)
            throw new ArgumentException("O aporte mensal deve ser de, no mínimo, R$ 100,00.", nameof(contribution));

        Name = name;
        Cpf = cpf;
        Email = email;
        MonthlyContribution = contribution;
        SubscriptionDate = DateTime.Now;
        IsActive = true;
    }

    public void UpdateContribution(decimal newContribution)
    {
        if (newContribution < 100)
            throw new ArgumentException("O aporte mensal deve ser de, no mínimo, R$ 100,00.", nameof(newContribution));
        MonthlyContribution = newContribution;
    }

    public void UpdateSubscriptionState(bool state) => IsActive = state;
   
}
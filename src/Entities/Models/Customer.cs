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
    public virtual ICollection<ContributionHistory> ContributionHistories { get; set; } = new List<ContributionHistory>();

    // EF
    protected Customer() { }

    public Customer(string name, string cpf, string email, decimal contribution)
    {
        cpf = new string(cpf.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(name))
            throw new CustomException("NAME_REQUIRED");

        if (!IsValidCpf(cpf))
            throw new CustomException("INVALID_CPF");

        if (contribution < 100)
            throw new CustomException("INVALID_MONTHLY_CONTRIBUTION");

        Name = name;
        Cpf = cpf;
        Email = email;
        MonthlyContribution = contribution;
        SubscriptionDate = DateTime.Now;
        IsActive = true;
    }

    private bool IsValidCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return false;

        cpf = new string(cpf.Where(char.IsDigit).ToArray());
        if (cpf.Length != 11) return false;
        if (cpf.Distinct().Count() == 1) return false;

        int sum = 0;

        for (int i = 0; i < 9; i++)
            sum += (cpf[i] - '0') * (10 - i);

        int remainder = sum % 11;
        int firstDigit = remainder < 2 ? 0 : 11 - remainder;

        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += (cpf[i] - '0') * (11 - i);

        remainder = sum % 11;
        int secondDigit = remainder < 2 ? 0 : 11 - remainder;

        return cpf[9] - '0' == firstDigit &&
            cpf[10] - '0' == secondDigit;
    }

    
    public void UpdateContribution(decimal newContribution)
    {
        if (newContribution < 100)
            throw new CustomException("VALOR_MENSAL_INVALIDO");
        MonthlyContribution = newContribution;
    }

    public void UpdateSubscriptionState(bool state) => IsActive = state;
   
}
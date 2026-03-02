using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TradingAccount
{
    public int Id { get; private set; }

    public int CustomerId { get; private set; }

    public string AccountNumber { get; private set; } = string.Empty;

    public AccountType Type { get; private set; }
    
    public Customer Customer { get; private set; } = null!;
    public List<Custody> Custodies { get; private set; } = new();

    // EF ctor
    protected TradingAccount() { }

    public TradingAccount(Customer customer, AccountType type)
    {
        Customer = customer;
        AccountNumber = GenerateAccountNumber();
        Type = type;
    }

    private string GenerateAccountNumber() 
    {
        return new Random().Next(100000, 999999).ToString() + "-1";
    }
}
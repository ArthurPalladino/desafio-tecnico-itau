using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TradingAccount
{
    public int Id { get; private set; }

    public int CustomerId { get; private set; }

    public string AccountNumber { get; private set; } = string.Empty;

    public decimal Balance { get; private set; }

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

    public void CreditBalance(decimal creditValue)
    {
        Balance += creditValue;
    }

    public void DebitBalance(decimal debitValue)
    {
        Balance -= debitValue;
    }


    public void AddCustody(string symbol, int quantity, decimal purchasePrice)
    {
        var existingCustody = Custodies.FirstOrDefault(c => c.Symbol == symbol);

        if (existingCustody != null)
        {
            existingCustody.AddQuantity(quantity, purchasePrice);
        }
        else
        {
            Custodies.Add(new Custody(this.CustomerId, this.Id, symbol, quantity, purchasePrice));
        }
    }

    public void RemoveCustody(string symbol, int quantity)
    {
        var custody = Custodies.FirstOrDefault(c => c.Symbol == symbol);
        
        if (custody == null) 
            throw new CustomException("CUSTODIA_INSUFICIENTE");

        custody.RemoveQuantity(quantity);

        if (custody.Quantity == 0)
        {
            Custodies.Remove(custody);
        }
    }
    private string GenerateAccountNumber() 
    {
        return new Random().Next(100000, 999999).ToString() + "-1";
    }

    
}
using System;
using FluentAssertions;
using Xunit;

public class TradingAccountTests
{
    [Fact]
    public void Constructor_ShouldSetTypeAndGenerateAccountNumber()
    {
        var customer = new Customer("Name", "12345678909", "test@example.com", 200);
        var account = new TradingAccount(customer, AccountType.Master);
        account.Type.Should().Be(AccountType.Master);
        account.AccountNumber.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void CreditAndDebitBalance_ShouldAdjustBalance()
    {
        var account = new TradingAccount(null, AccountType.Master);
        account.CreditBalance(100);
        account.Balance.Should().Be(100);
        account.DebitBalance(20);
        account.Balance.Should().Be(80);
    }

    [Fact]
    public void AddCustody_ShouldAddNewCustody_WhenNoneExists()
    {
        var account = new TradingAccount(null, AccountType.Master);
        account.GetType().GetProperty("Id")?.SetValue(account, 1);
        account.GetType().GetProperty("CustomerId")?.SetValue(account, 999);

        account.AddCustody("SYM", 5, 10);

        account.Custodies.Should().ContainSingle(c => c.Symbol == "SYM" && c.Quantity == 5);
    }

    [Fact]
    public void AddCustody_ShouldIncreaseExistingQuantity()
    {
        var account = new TradingAccount(null, AccountType.Master);
        account.GetType().GetProperty("Id")?.SetValue(account, 1);
        account.GetType().GetProperty("CustomerId")?.SetValue(account, 999);

        account.AddCustody("SYM", 5, 10);
        account.AddCustody("SYM", 3, 10);

        account.Custodies.Should().ContainSingle(c => c.Quantity == 8);
    }

    [Fact]
    public void RemoveCustody_ShouldRemoveQuantityOrThrow()
    {
        var account = new TradingAccount(null, AccountType.Master);
        account.GetType().GetProperty("Id")?.SetValue(account, 1);
        account.GetType().GetProperty("CustomerId")?.SetValue(account, 999);

        account.AddCustody("SYM", 5, 10);
        account.RemoveCustody("SYM", 3);

        account.Custodies.Should().ContainSingle(c => c.Quantity == 2);
    }
    [Fact]
    public void RemoveCustody_ShouldThrow_WhenInsufficient()
    {
        var account = new TradingAccount(null, AccountType.Master);
        Action act = () => account.RemoveCustody("SYM", 1);
        act.Should().Throw<CustomException>().WithMessage("Quantidade insuficiente em custódia.");
    }
}

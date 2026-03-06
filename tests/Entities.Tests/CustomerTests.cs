using System;
using FluentAssertions;
using Xunit;

public class CustomerTests
{
    [Fact]
    public void Constructor_ShouldCreateEntity_WhenDataIsNominal()
    {
        var customer = new Customer("Name", "123.456.789-09", "test@example.com", 200);
        customer.Name.Should().Be("Name");
        customer.Cpf.Should().Be("12345678909");
        customer.Email.Should().Be("test@example.com");
        customer.MonthlyContribution.Should().Be(200);
        customer.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenCpfIsInvalid()
    {
        Action act = () => new Customer("Name", "11111111111", "test@example.com", 200);
        act.Should().Throw<CustomException>().WithMessage("CPF inválido. O CPF fornecido não é um CPF real.");
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenEmailIsInvalid()
    {
        Action act = () => new Customer("Name", "12345678909", "invalid", 200);
        act.Should().Throw<CustomException>().WithMessage("Email inválido. O email fornecido não é válido.");
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenNameIsEmpty()
    {
        Action act = () => new Customer("", "12345678909", "test@example.com", 200);
        act.Should().Throw<CustomException>().WithMessage("O nome é obrigatório.");
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenContributionTooLow()
    {
        Action act = () => new Customer("Name", "12345678909", "test@example.com", 50);
        act.Should().Throw<CustomException>().WithMessage("O valor mensal minimo e de R$ 100,00.");
    }

    [Fact]
    public void UpdateContribution_ShouldUpdateValue_WhenAmountIsValid()
    {
        var customer = new Customer("Name", "12345678909", "test@example.com", 200);
        customer.UpdateContribution(300);
        customer.MonthlyContribution.Should().Be(300);
    }

    [Fact]
    public void UpdateContribution_ShouldThrowException_WhenAmountInvalid()
    {
        var customer = new Customer("Name", "12345678909", "test@example.com", 200);
        Action act = () => customer.UpdateContribution(50);
        act.Should().Throw<CustomException>().WithMessage("O valor mensal minimo e de R$ 100,00.");
    }

    [Fact]
    public void UpdateSubscriptionState_ShouldChangeState()
    {
        var customer = new Customer("Name", "12345678909", "test@example.com", 200);
        customer.UpdateSubscriptionState(false);
        customer.IsActive.Should().BeFalse();
    }
}

using Repositories.Interfaces;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ITradingAccountRepository _tradingAccountRepository;

    public CustomerService(ICustomerRepository customerRepository, ITradingAccountRepository tradingAccountRepository)
    {
        _customerRepository = customerRepository;
        _tradingAccountRepository = tradingAccountRepository;
    }

    public async Task<int> CreateAsync(string name, string cpf, string email, decimal contribution)
    {
        var existing = await _customerRepository.GetCustomerByCpf(cpf);
        if (existing != null) throw new InvalidOperationException("CPF já cadastrado.");

        Customer customer = new Customer(name, cpf, email,contribution);
        TradingAccount tradingAccount = new TradingAccount(customer,AccountType.SubAccount);

        await _customerRepository.AddAsync(customer);
        await _tradingAccountRepository.AddAsync(tradingAccount);
        await _customerRepository.SaveChangesAsync();

        return customer.Id;
    }

    public async Task UpdateMonthlyAmountAsync(int customerId, decimal newAmount)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null || !customer.IsActive) 
            throw new InvalidOperationException("Uma assinatura ativa não foi encontrada.");

        customer.UpdateContribution(newAmount);

        await _customerRepository.SaveChangesAsync();
    }

    public async Task UpdateSubscriptionState(int customerId, bool state)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null) throw new KeyNotFoundException("Usuário näo encontrado");

        customer.UpdateSubscriptionState(state);
        await _customerRepository.SaveChangesAsync();
    }

    public async Task<object> GetPortfolioSummaryAsync(int customerId)
    {
        var customer = await _customerRepository.GetCustomerWithPortfolioAsync(customerId);
        if (customer == null) throw new KeyNotFoundException("Usuário näo encontrado.");

        return new 
        {
            customer.Name,
            customer.MonthlyContribution,
            IsActive = customer.IsActive,
            Positions = customer.Custodies 
        };
    }

    public async Task<PortfolioProfitabilityResponse> GetDetailedProfitabilityAsync(int customerId)
    {
        var customer = await _customerRepository.GetCustomerWithPortfolioAsync(customerId);
        if (customer == null) throw new KeyNotFoundException("Cliente não encontrado.");

        var account = customer.TradingAccount;
        var assets = new List<AssetProfitabilityDto>();

        foreach (var item in account.Custodies)
        {
            decimal currentPrice = item.AveragePrice * 1.05m;
            
            var profitLoss = (currentPrice - item.AveragePrice) * item.Quantity;
            var variation = ((currentPrice / item.AveragePrice) - 1) * 100;

            assets.Add(new AssetProfitabilityDto(
                item.Symbol,
                item.Quantity,
                item.AveragePrice,
                currentPrice,
                profitLoss,
                variation
            ));
        }

        decimal totalInvested = account.Custodies.Sum(x => x.Quantity * x.AveragePrice);
        decimal currentTotalValue = assets.Sum(x => x.Quantity * x.CurrentPrice);

        return new PortfolioProfitabilityResponse(
            customer.Name,
            totalInvested,
            currentTotalValue,
            currentTotalValue - totalInvested,
            totalInvested > 0 ? ((currentTotalValue / totalInvested) - 1) * 100 : 0,
            assets
        );
    }
}
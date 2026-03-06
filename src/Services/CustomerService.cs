using Repositories.Interfaces;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ITradingAccountRepository _tradingAccountRepository;
    private readonly IContributionHistoryRepository _historyRepository;
    private readonly ITickerRepository _tickerRepository;

    public CustomerService(ICustomerRepository customerRepository,
    ITradingAccountRepository tradingAccountRepository,
    IContributionHistoryRepository historyRepository,
    ITickerRepository tickerRepository)
    {
        _customerRepository = customerRepository;
        _tradingAccountRepository = tradingAccountRepository;
        _historyRepository = historyRepository;
        _tickerRepository = tickerRepository;
    }

    public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request)
    {
        if (request.ValorMensal < 100) 
            throw new CustomException("VALOR_MENSAL_INVALIDO");

        var existing = await _customerRepository.GetCustomerByCpf(request.Cpf);
        if (existing != null) 
            throw new CustomException("CLIENTE_CPF_DUPLICADO");

        var customer = new Customer(request.Nome, request.Cpf, request.Email, request.ValorMensal);
        var tradingAccount = new TradingAccount(customer, AccountType.SubAccount);

        var history = new ContributionHistory(customer,0,request.ValorMensal);


        await _historyRepository.AddAsync(history);
        await _customerRepository.AddAsync(customer);
        await _tradingAccountRepository.AddAsync(tradingAccount);
        await _customerRepository.SaveChangesAsync();

        return new CustomerResponse(
            customer.Id,
            customer.Name,
            customer.Cpf,
            customer.Email,
            customer.MonthlyContribution,
            customer.IsActive,
            DateTime.UtcNow,
            new GraphicAccountResponse(tradingAccount.Id, $"FLH-{customer.Id:D6}", "FILHOTE", DateTime.UtcNow)
        );
    }

    public async Task<SubscriptionChangeResponse> LeaveInvestmentProductAsync(int customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null) 
            throw new CustomException("CLIENTE_NAO_ENCONTRADO");
        
        if (!customer.IsActive)
            throw new CustomException("CLIENTE_JA_INATIVO");

        customer.UpdateSubscriptionState(false);
        await _customerRepository.SaveChangesAsync();

        return new SubscriptionChangeResponse(
            customer.Id,
            customer.Name,
            false,
            DateTime.UtcNow,
            "Adesao encerrada. Sua posicao em custodia foi mantida."
        );
    }

    public async Task<UpdateAmountResponse> UpdateMonthlyAmountAsync(int customerId, decimal newValue)
    {
        if (newValue < 100) 
            throw new CustomException("VALOR_MENSAL_INVALIDO");

        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null) 
            throw new CustomException("CLIENTE_NAO_ENCONTRADO");

        var odlValue = customer.MonthlyContribution;

        if (odlValue == newValue)
        {
            throw new CustomException("VALOR_APORTE_IDENTICO");
        }

        var history = new ContributionHistory(customer,odlValue, newValue);


        await _historyRepository.AddAsync(history);
        customer.UpdateContribution(newValue);
        await _customerRepository.SaveChangesAsync();

        return new UpdateAmountResponse(
            customer.Id,
            odlValue,
            customer.MonthlyContribution,
            DateTime.UtcNow,
            "Valor mensal atualizado. O novo valor sera considerado a partir da proxima data de compra."
        );
    }

    public async Task<PortfolioSummaryResponse> GetPortfolioSummaryAsync(int customerId)
    {
        var customer = await _customerRepository.GetCustomerWithPortfolioAsync(customerId);
        if (customer == null) 
            throw new CustomException("CLIENTE_NAO_ENCONTRADO");

        var account = customer.TradingAccount;
        
        var symbols = account.Custodies.Select(c => c.Symbol).ToList();
        var currentPrices = await _tickerRepository.GetTickersDictBySymbol(symbols);

        decimal totalInvestedValue = account.Custodies.Sum(x => x.Quantity * x.AveragePrice);
        
        decimal currentPortfolioValue = account.Custodies.Sum(x => {
            var tickerInfo = currentPrices.GetValueOrDefault(x.Symbol);
            decimal price = tickerInfo?.CurrentPrice ?? x.AveragePrice;
            return x.Quantity * price;
        });

        var assets = account.Custodies.Select(x => {
            var tickerInfo = currentPrices.GetValueOrDefault(x.Symbol);
            decimal marketPrice = tickerInfo?.CurrentPrice ?? x.AveragePrice;
            decimal positionValue = x.Quantity * marketPrice;
            decimal assetProfitability = x.AveragePrice > 0 
            ? ((marketPrice / x.AveragePrice) - 1) * 100 
            : 0;

            return new AssetDto(
                x.Symbol,
                x.Quantity,
                x.AveragePrice,
                marketPrice, 
                Math.Round(positionValue,2), 
                positionValue - (x.Quantity * x.AveragePrice), 
                assetProfitability, 
                currentPortfolioValue > 0 ? (positionValue / currentPortfolioValue) * 100 : 0
            );
        }).ToList();
        return new PortfolioSummaryResponse(
            customer.Id,
            customer.Name,
            $"FLH-{customer.Id:D6}",
            DateTime.UtcNow,
            new PortfolioMetrics(
                totalInvestedValue,
                currentPortfolioValue,
                currentPortfolioValue - totalInvestedValue,
                totalInvestedValue > 0 ? ((currentPortfolioValue / totalInvestedValue) - 1) * 100 : 0
            ),
            assets
        );
    }

    public async Task<PortfolioProfitabilityResponse> GetDetailedProfitabilityAsync(int customerId)
    {
        var customer = await _customerRepository.GetCustomerWithPortfolioAsync(customerId);
        if (customer == null) 
            throw new CustomException("CLIENTE_NAO_ENCONTRADO");
        
        var account = customer.TradingAccount;
        var symbols = account.Custodies.Select(x => x.Symbol).ToList();
        var currentPrices = await _tickerRepository.GetTickersDictBySymbol(symbols);

        decimal totalInvested = account.Custodies.Sum(x => x.Quantity * x.AveragePrice);
        decimal currentTotalValue = account.Custodies.Sum(x => 
        {
            decimal precoAtual = currentPrices.GetValueOrDefault(x.Symbol)?.CurrentPrice ?? x.AveragePrice;            
            return x.Quantity * precoAtual;
        });

        



        return new PortfolioProfitabilityResponse(
            customer.Id,
            customer.Name,
            DateTime.UtcNow,
            new PortfolioMetrics(
                totalInvested,
                currentTotalValue,
                currentTotalValue - totalInvested,
                totalInvested > 0 ? ((currentTotalValue / totalInvested) - 1) * 100 : 0
            ),
            new List<AporteDto>(),
            new List<EvolucaoDto>()  
        );
    }
}
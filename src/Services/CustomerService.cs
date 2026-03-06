using Repositories.Interfaces;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ITradingAccountRepository _tradingAccountRepository;
    private readonly IContributionHistoryRepository _historyRepository;
    private readonly ITickerRepository _tickerRepository;

    private readonly IDistributionRepository _distributionRepository;
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;

    public CustomerService(ICustomerRepository customerRepository,
    ITradingAccountRepository tradingAccountRepository,
    IContributionHistoryRepository historyRepository,
    ITickerRepository tickerRepository,
    IPurchaseOrderRepository purchaseOrderRepository,
    IDistributionRepository distributionRepository)
    {
        _distributionRepository = distributionRepository;
        _purchaseOrderRepository = purchaseOrderRepository;
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

        var response = new PortfolioProfitabilityResponse(
            customer.Id,
            customer.Name,
            DateTime.UtcNow,
            new PortfolioMetrics(
                totalInvested,
                currentTotalValue,
                currentTotalValue - totalInvested,
                totalInvested > 0 ? ((currentTotalValue / totalInvested) - 1) * 100 : 0
            ),
            await GetContributionHistory(customer),
            await GetEvolutionPatrimony(customer)  
        );
        return response;
    }

    private async Task<List<AporteDto>> GetContributionHistory(Customer customer)
    {
        var history = await _historyRepository.GetByCustomerIdAsync(customer.Id);
        var distributions = await _distributionRepository.GetAllDistributionsToChildAccount(customer.TradingAccount.Id);
        
        var contributionDays = distributions
            .Select(d => d.DistributedAt.Date)
            .Distinct()
            .OrderBy(date => date)
            .ToList();

        var aportesFinal = new List<AporteDto>();

        var groupedByMonth = contributionDays
            .GroupBy(d => new { d.Year, d.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month);

        foreach (var monthGroup in groupedByMonth)
        {
            var daysInMonth = monthGroup.ToList();
            
            for (int i = 0; i < daysInMonth.Count; i++)
            {
                var currentDay = daysInMonth[i];

                var activeRule = history
                    .Where(h => h.AlterationDate.Date <= currentDay)
                    .OrderByDescending(h => h.AlterationDate)
                    .FirstOrDefault();

                decimal contributionAmount = activeRule?.NewValue ?? customer.MonthlyContribution;
                
                int installmentNumber = (i % 3) + 1;

                aportesFinal.Add(new AporteDto(
                    currentDay.ToString("yyyy-MM-dd"),
                    Math.Round(contributionAmount, 2),
                    $"{installmentNumber}/3"
                ));
            }
        }

        return aportesFinal;
    }

    private async Task<List<EvolucaoDto>> GetEvolutionPatrimony(Customer customer)
    {
        TradingAccount account = customer.TradingAccount;
        var distributions = await _distributionRepository.GetAllDistributionsToChildAccount(account.Id);
        var purchaseOrders = await _purchaseOrderRepository.GetAllByBuyerId(account.Id);

        var timeline = distributions
        .Select(d => new { d.DistributedAt.Date, Amount = (decimal)d.Quantity }) 
        .AsEnumerable() 
        .Concat(purchaseOrders.Select(o => new { o.ExecutionDate.Date, Amount = o.Quantity * o.UnitPrice }))
        .GroupBy(x => x.Date)
        .OrderBy(g => g.Key)
        .Select(g => new { 
            Date = g.Key, 
            DailyNetChange = g.Sum(x => x.Amount) 
        })
        .ToList();

        
        var finalEvolution = new List<EvolucaoDto>();
        decimal runningTotalInvested = 0;

        foreach (var item in timeline)
        {
            runningTotalInvested += item.DailyNetChange;
            
            decimal marketValueOnDate = 0;
            foreach (var asset in customer.TradingAccount.Custodies) 
            {
                var historicPrice = await _tickerRepository.GetPriceAtDate(asset.Symbol, item.Date);
                
                decimal price = historicPrice != null ? historicPrice.CurrentPrice : asset.AveragePrice;
                marketValueOnDate += asset.Quantity * price;
            }

            finalEvolution.Add(new EvolucaoDto(
                item.Date.ToString("yyyy-MM-dd"),
                Math.Round(marketValueOnDate, 2),
                Math.Round(runningTotalInvested, 2),
                runningTotalInvested > 0 ? Math.Round(((marketValueOnDate / runningTotalInvested) - 1) * 100, 2) : 0
            ));
        }

        return finalEvolution;
    }
}
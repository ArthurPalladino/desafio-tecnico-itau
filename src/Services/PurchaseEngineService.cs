using Repositories.Interfaces;

public class PurchaseEngineService : IPurchaseEngineService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ITaxEventRepository _taxEventRepository;
    private readonly IDistributionRepository _distributionRepository;
    private readonly IRecommendationBasketRepository _basketRepository;
    private readonly ITickerRepository _tickerRepository;
    private readonly ITradingAccountRepository _accountRepository;
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly ITaxService _taxService;
    private readonly IKafkaProducer _kafkaProducer;

    public PurchaseEngineService(
        IPurchaseOrderRepository purchaseOrderRepository,
        ICustomerRepository customerRepository,
        IRecommendationBasketRepository basketRepository,
        ITickerRepository tickerRepository,
        ITradingAccountRepository accountRepository,
        ITaxService taxService,
        IDistributionRepository distributionRepository,
        IKafkaProducer kafkaProducer,
        ITaxEventRepository taxEventRepository
        )
    {
        _taxEventRepository = taxEventRepository;
        _purchaseOrderRepository = purchaseOrderRepository;
        _customerRepository = customerRepository;
        _basketRepository = basketRepository;
        _tickerRepository = tickerRepository;
        _accountRepository = accountRepository;
        _taxService = taxService;
        _kafkaProducer = kafkaProducer;
        _distributionRepository = distributionRepository;
    }

    private DateTime ReturnUtilDate(DateTime data)
{
    if (data.DayOfWeek == DayOfWeek.Saturday)
    {
        return data.AddDays(2);
    }
    
    if (data.DayOfWeek == DayOfWeek.Sunday)
    {
        return data.AddDays(1);
    }

    return data;
}
    async Task<int> TickerDistribution(IEnumerable<Customer> customers, Ticker ticker, Custody masterCustody, Dictionary<int, decimal> percentagePerCustomer, PurchaseOrder purchaseOrder, List<Distribution> allDistributions)
    {
        int events = 0;
        foreach (var customer in customers)
        {
            decimal customerPercentage = percentagePerCustomer[customer.Id];
            decimal customerQuantityDec = customerPercentage * masterCustody.Quantity;
            int customerQuantity = Convert.ToInt32(Math.Floor(customerQuantityDec));
            if (customerQuantity > 0)
            {
                var distribution = new Distribution(purchaseOrder, customer.TradingAccount.Id, ticker.Symbol, customerQuantity, purchaseOrder.UnitPrice);
                await _distributionRepository.AddAsync(distribution);
                allDistributions.Add(distribution);
                
                await PostIrInKafkaAsync(customer, ticker, customerQuantity);
                events++;
                
                customer.TradingAccount.AddCustody(ticker.Symbol, customerQuantity, ticker.CurrentPrice);
                masterCustody.RemoveQuantity(customerQuantity);
            }
        }
        return events;
    }

    public async Task PostIrInKafkaAsync(Customer customer, Ticker ticker, decimal qty)
    {
        decimal valorOperacao = qty * ticker.CurrentPrice;
        decimal valorIR = _taxService.CalculateRetentionTax(valorOperacao);
        decimal aliquota = _taxService.RetentionTaxRate;
        TaxEvent taxEvent = new TaxEvent(customer.Id, valorOperacao, valorIR, TaxType.DedoDuro);
        await _taxEventRepository.AddAsync(taxEvent);
        var irDedoDuroMessage = new
        {
            tipo = "IR_DEDO_DURO",
            clienteId = customer.Id,
            cpf = customer.Cpf,
            ticker = ticker.Symbol,
            tipoOperacao = "COMPRA",
            quantidade = qty,
            precoUnitario = ticker.CurrentPrice,
            valorOperacao = valorOperacao,
            aliquota = aliquota,
            valorIR = valorIR,
            dataOperacao = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };

        await _kafkaProducer.ProduceAsync("ir-dedo-duro", customer.Id.ToString(), irDedoDuroMessage);
        taxEvent.PublicInKafka();
    }

    public async Task<List<PurchaseOrder>> CreatePurchaseOrder(int masterAccountId, string tickerBase, int quantidadeTotal, decimal unitPrice)
    {
        int lotePadrao = quantidadeTotal / 100 * 100;
        int fracionario = quantidadeTotal % 100;

        var createdOrders = new List<PurchaseOrder>();
        if (fracionario > 0)
        {
            var pFrac = new PurchaseOrder(masterAccountId, tickerBase + "F", MarketType.Fractional, fracionario, unitPrice);
            await _purchaseOrderRepository.AddAsync(pFrac);
            createdOrders.Add(pFrac);
        }

        if (lotePadrao > 0)
        {
            var pLote = new PurchaseOrder(masterAccountId, tickerBase, MarketType.StandardLot, lotePadrao, unitPrice);
            await _purchaseOrderRepository.AddAsync(pLote);
            createdOrders.Add(pLote);
        }
        return createdOrders;
    }

    public async Task<MotorCompraResponseDto> ExecuteAsync(DateTime referDate)
    {
        referDate = ReturnUtilDate(referDate);

        var customers = await _customerRepository.GetActiveCustomers();
        if (customers == null || !customers.Any()) 
            throw new CustomException("NENHUM_CLIENTE_ATIVO");

        var recommendationBasket = await _basketRepository.GetActiveBasketWithItensAsync();
        if (recommendationBasket == null) 
            throw new CustomException("CESTA_NAO_ENCONTRADA");

        if (recommendationBasket.Itens == null || recommendationBasket.Itens.Count != 5)
            throw new CustomException("QUANTIDADE_ATIVOS_INVALIDA");

        var symbols = recommendationBasket.Itens.Select(c => c.Symbol).ToList();
        var tickers = await _tickerRepository.GetTickersDictBySymbol(symbols);
        
        if (tickers == null || tickers.Count < symbols.Count)
            throw new CustomException("COTACAO_NAO_ENCONTRADA");

        var masterAccount = await _accountRepository.GetMasterAccount();
        if (masterAccount == null) 
            throw new CustomException("CONTA_MASTER_INVALIDA");


        var totalAmountWithNoMasterBalance = customers.Sum(c => c.MonthlyContribution / 3m);
        var totalAmount = totalAmountWithNoMasterBalance + masterAccount.Balance;

        masterAccount.CreditBalance(totalAmountWithNoMasterBalance);
        Dictionary<int, decimal> percentagePerCustomer = customers.ToDictionary(c => c.Id, c => c.MonthlyContribution / 3m / totalAmountWithNoMasterBalance);

        var allCreatedOrders = new List<PurchaseOrder>();
        var allCreatedDistributions = new List<Distribution>();
        int totalEvents = 0;
        var residues = new List<ResiduoMasterDto>();

        foreach (var item in recommendationBasket.Itens)
        {
            Ticker ticker = tickers[item.Symbol];
            Custody masterCustody = masterAccount.Custodies.FirstOrDefault(c => c.Symbol == ticker.Symbol);
            int totalNeeded = (int)Math.Floor(totalAmount * (item.Percentage / 100m) / ticker.CurrentPrice);
            int availableInMaster = masterCustody?.Quantity ?? 0;
            int quantityToBuy = Math.Max(0, totalNeeded - availableInMaster);

            PurchaseOrder currentOrderRef = null;
            if (quantityToBuy > 0)
            {
                var orders = await CreatePurchaseOrder(masterAccount.Id, ticker.Symbol, quantityToBuy, ticker.CurrentPrice);
                allCreatedOrders.AddRange(orders);
                currentOrderRef = orders.Last();

                masterAccount.DebitBalance(quantityToBuy * ticker.CurrentPrice);
                masterAccount.AddCustody(ticker.Symbol, quantityToBuy, ticker.CurrentPrice);
                masterCustody = masterAccount.Custodies.FirstOrDefault(c => c.Symbol == ticker.Symbol);
            }

            if (masterCustody != null && masterCustody.Quantity > 0)
            {
                currentOrderRef ??= await _purchaseOrderRepository.GetLastOrderAsync(ticker.Symbol) 
                       ?? throw new CustomException("ORDEM_COMPRA_INVALIDA");
                totalEvents += await TickerDistribution(customers, ticker, masterCustody, percentagePerCustomer, currentOrderRef, allCreatedDistributions);
                
                if(masterCustody.Quantity > 0)
                {
                    residues.Add(new ResiduoMasterDto { Ticker = ticker.Symbol, Quantidade = masterCustody.Quantity });
                }
            }
        }

        await _customerRepository.SaveChangesAsync();

        return new MotorCompraResponseDto
        {
            DataExecucao = referDate,
            TotalClientes = customers.Count(),
            TotalConsolidado = Math.Round(totalAmountWithNoMasterBalance,2),
            Mensagem = $"Compra programada executada com sucesso para {customers.Count()} cliente{(customers.Count() > 1 ? "s" : "")}.",
            EventosIRPublicados = totalEvents,
            ResiduosCustMaster = residues,
            OrdensCompra = allCreatedOrders.GroupBy(o => o.Symbol.Replace("F", ""))
                .Select(g => new OrdemResumoDto
                {
                    Ticker = g.Key,
                    QuantidadeTotal = g.Sum(x => x.Quantity),
                    PrecoUnitario = g.First().UnitPrice,
                    ValorTotal = g.Sum(x => x.Quantity * x.UnitPrice),
                    Detalhes = g.Select(d => new OrdemDetalheDto { 
                        Tipo = d.MarketType.ToString().ToUpper(), 
                        Ticker = d.Symbol, 
                        Quantidade = d.Quantity 
                    }).ToList()
                }).ToList(),
            Distribuicoes = customers.Select(c => new DistribuicaoAgrupadaDto
            {
                ClienteId = c.Id,
                Nome = c.Name,
                ValorAporte = Math.Round(c.MonthlyContribution / 3m,2),
                Ativos = allCreatedDistributions.Where(d => d.ChildAccountId == c.TradingAccount.Id)
                    .Select(a => new AtivoDistribuidoDto { Ticker = a.Symbol, Quantidade = a.Quantity }).ToList()
            }).ToList()
        };
    }
}
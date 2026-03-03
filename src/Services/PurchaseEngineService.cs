
using ItauCompraProgramada.Application.Interfaces;
using Repositories.Interfaces;

public class PurchaseEngineService : IPurchaseEngineService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IRecommendationBasketRepository _basketRepository;
    private readonly ITickerRepository _tickerRepository;
    private readonly ITradingAccountRepository _accountRepository;
    private readonly ICustodyRepository _custodyRepository;

    private readonly IPurchaseOrderRepository _purchaseOrderRepository;

    public PurchaseEngineService(
        IPurchaseOrderRepository purchaseOrderRepository,
        ICustomerRepository customerRepository,
        IRecommendationBasketRepository basketRepository,
        ITickerRepository tickerRepository,
        ITradingAccountRepository accountRepository,
        ICustodyRepository custodyRepository
        )
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _custodyRepository = custodyRepository;
        _customerRepository = customerRepository;
        _basketRepository = basketRepository;
        _tickerRepository = tickerRepository;
        _accountRepository = accountRepository;
    }

    void TickerDistribution(IEnumerable<Customer> customers, Ticker ticker, Custody masterCustody, Dictionary<int, decimal> percentagePerCustomer)
    {
        foreach (var customer in customers)
        {
            decimal customerPercentage = percentagePerCustomer[customer.Id];
            decimal customerQuantityDec = customerPercentage * masterCustody.Quantity;
            int customerQuantity = Convert.ToInt32(Math.Floor(customerQuantityDec));
            if(customerQuantity>0){
                customer.TradingAccount.AddCustody(ticker.Symbol,customerQuantity,ticker.CurrentPrice);
                masterCustody.RemoveQuantity(customerQuantity);
            }
        }            
    }

    public async Task CreatePurchaseOrder(int masterAccountId, string tickerBase, int quantidadeTotal, decimal unitPrice)
    {
        int lotePadrao = (quantidadeTotal / 100) * 100; 
        int fracionario = quantidadeTotal % 100;      

        if (lotePadrao > 0)
        {
           await _purchaseOrderRepository.AddAsync(new PurchaseOrder(masterAccountId,tickerBase, MarketType.StandardLot, lotePadrao, unitPrice));
        }

        if (fracionario > 0)
        {
            await _purchaseOrderRepository.AddAsync(new PurchaseOrder(masterAccountId,tickerBase + "F", MarketType.Fractional, fracionario, unitPrice));
        }
    }

    public async Task ExecuteAsync(DateTime referDate)
    {
        var customers = await _customerRepository.GetActiveCustomers();
        var recommendationBasket = await _basketRepository.GetActiveBasketWithItensAsync();
        var tickers = await _tickerRepository.GetTickersByDateDictAsync(referDate.Date);
        var masterAccount = await _accountRepository.GetMasterAccount(); 

        var totalAmountWithNoMasterBalance = customers.Sum(c => c.MonthlyContribution/3m);
        var totalAmount = totalAmountWithNoMasterBalance + masterAccount.Balance;

        masterAccount.CreditBalance(totalAmountWithNoMasterBalance);
        Dictionary<int, decimal> percentagePerCustomer = customers.ToDictionary(c => c.Id,c => c.MonthlyContribution / 3m / totalAmountWithNoMasterBalance);
        foreach(var item in recommendationBasket.Itens)
        {
            Ticker ticker = tickers[item.Symbol];
            
            Custody masterCustody = masterAccount.Custodies.FirstOrDefault(c => c.Symbol == ticker.Symbol);
            int totalNeeded = (int)Math.Floor(totalAmount * (item.Percentage / 100m) / ticker.CurrentPrice);
            int availableInMaster = masterCustody?.Quantity ?? 0;
            int quantityToBuy = Math.Max(0, totalNeeded - availableInMaster);

            if(quantityToBuy > 0)
            {
                await CreatePurchaseOrder(masterAccount.Id,ticker.Symbol,quantityToBuy,ticker.CurrentPrice);
                masterAccount.DebitBalance(quantityToBuy*ticker.CurrentPrice);
                masterAccount.AddCustody(ticker.Symbol,quantityToBuy,ticker.CurrentPrice);
                masterCustody = masterAccount.Custodies.FirstOrDefault(c => c.Symbol == ticker.Symbol);
            }
            if (masterCustody != null && masterCustody.Quantity > 0)
            {
                TickerDistribution(customers, ticker, masterCustody, percentagePerCustomer);
            }
        }
        await _customerRepository.SaveChangesAsync();
    }

    
}


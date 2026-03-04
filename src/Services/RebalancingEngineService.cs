using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.OpenApi.Services;
using Repositories.Interfaces;

public class RebalancingEngineService : IRebalancingEngineService
{
    const decimal THRESHOLD = 0.05m;
    private readonly ICustomerRepository _customerRepository;
    private readonly ITaxEventRepository _taxEventRepository;
    private readonly IDistributionRepository _distributionRepository;
    private readonly IRecommendationBasketRepository _basketRepository;
    private readonly ITickerRepository _tickerRepository;
    private readonly ITradingAccountRepository _accountRepository;
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly ITaxService _taxService;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IPurchaseOrderService _purchaseOrderService;

    public RebalancingEngineService(
        IPurchaseOrderRepository purchaseOrderRepository,
        ICustomerRepository customerRepository,
        IRecommendationBasketRepository basketRepository,
        ITickerRepository tickerRepository,
        ITradingAccountRepository accountRepository,
        ITaxService taxService,
        IDistributionRepository distributionRepository,
        IKafkaProducer kafkaProducer,
        ITaxEventRepository taxEventRepository,
        IPurchaseOrderService purchaseOrderService
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
        _purchaseOrderService = purchaseOrderService;
    }
    

    async Task<decimal> Sell(TradingAccount account, Custody custody,int quantity,Ticker ticker)
    {
        account.CreditBalance(quantity*ticker.CurrentPrice);
        account.RemoveCustody(custody.Symbol,quantity);
        await _purchaseOrderService.CreateSellOrders(account,custody,ticker.CurrentPrice);
        return ticker.CurrentPrice*quantity;
    }
    public async Task MasterRebalancing(TradingAccount masterAccount, List<BasketItem> itens,Dictionary<string,Ticker> tickers)
    {
        var masterCustodies = masterAccount.Custodies.ToList();
        foreach (var masterCustody in masterCustodies)
        {
            if (!itens.Any(i => i.Symbol == masterCustody.Symbol))
            {
                await Sell(masterAccount, masterCustody, masterCustody.Quantity,tickers[masterCustody.Symbol]);
            }
        }
    }
    public async Task<bool> ExecuteAsync(RebalancingType rebalancingType)
    {
        var customers = await _customerRepository.GetActiveCustomers();
        if (customers == null || !customers.Any()) 
            return false;

        var recommendationBasket = await _basketRepository.GetActiveBasketWithItensAsync();
        if (recommendationBasket == null) 
            throw new CustomException("CESTA_NAO_ENCONTRADA");

        var symbols = recommendationBasket.Itens.Select(c => c.Symbol).ToList();
        var tickers = await _tickerRepository.GetTickersDictByLastDate();
        
        if (tickers == null)
            throw new CustomException("COTACAO_NAO_ENCONTRADA");

        var masterAccount = await _accountRepository.GetMasterAccount();
        if (masterAccount == null) 
            throw new CustomException("CONTA_MASTER_INVALIDA");
        

        if(rebalancingType == RebalancingType.RecommendationChange)
        {            
            await MasterRebalancing(masterAccount, recommendationBasket.Itens,tickers);
        }
       

        foreach (var customer in customers)
        {
            decimal currentWealth = customer.TradingAccount.Balance + 
                                    customer.TradingAccount.Custodies.Sum(c => c.Quantity * tickers[c.Symbol].CurrentPrice);

            var currentCustodies = customer.TradingAccount.Custodies.ToList();
            foreach (var customerCustody in currentCustodies)
            {
                var basketItem = recommendationBasket.Itens.FirstOrDefault(i => i.Symbol == customerCustody.Symbol);
                var ticker = tickers[customerCustody.Symbol];

                int quantityToSell = 0;
                decimal currentWeight = (customerCustody.Quantity * ticker.CurrentPrice) / currentWealth;
                decimal targetWeight = basketItem != null ? (basketItem.Percentage / 100m) : 0m;
                if (basketItem == null) quantityToSell = customerCustody.Quantity;
                else if (currentWeight > (targetWeight + THRESHOLD))
                {
                    decimal targetValue = targetWeight * currentWealth;
                    decimal overValue = (customerCustody.Quantity * ticker.CurrentPrice) - targetValue;
                    quantityToSell = (int)Math.Floor(overValue / ticker.CurrentPrice);
                }

                if (quantityToSell > 0)
                {
                    decimal operationValue = await Sell(customer.TradingAccount,customerCustody,quantityToSell,ticker);
                    DateTime now = DateTime.Now;
                    decimal totalSalesInMonth = await _purchaseOrderRepository.GetTotalSalesValueInMonthAsync(customer.TradingAccount.Id,now.Year,now.Month);
                    var tax = _taxService.CalculateCapitalGainTax(operationValue,customerCustody.AveragePrice,quantityToSell,totalSalesInMonth);
                    await _taxService.PostTaxInKafkaAsync(customer,ticker.Symbol,operationValue,tax,TaxType.Venda);
                }
            }

            currentWealth = customer.TradingAccount.Balance + 
                            customer.TradingAccount.Custodies.Sum(c => c.Quantity * tickers[c.Symbol].CurrentPrice);

            foreach (var item in recommendationBasket.Itens)
            {
                var ticker = tickers[item.Symbol];
                var customerCustody = customer.TradingAccount.Custodies.FirstOrDefault(c => c.Symbol == item.Symbol);

                decimal currentWeight = customerCustody != null 
                    ? (customerCustody.Quantity * ticker.CurrentPrice) / currentWealth 
                    : 0;
                
                decimal targetWeight = item.Percentage / 100m;

                if (currentWeight < (targetWeight - THRESHOLD))
                {
                    decimal targetValue = targetWeight * currentWealth;
                    decimal currentValue = currentWeight * currentWealth;
                    decimal neededValue = targetValue - currentValue;

                    decimal finalInvestment = Math.Min(neededValue, customer.TradingAccount.Balance);
                    int quantityToBuy = (int)Math.Floor(finalInvestment / ticker.CurrentPrice);

                    if (quantityToBuy > 0)
                    {
                            var orders = await _purchaseOrderService.CreatePurchaseOrder(customer.TradingAccount.Id, ticker.Symbol, quantityToBuy, ticker.CurrentPrice);
                            decimal buyOperationValue = quantityToBuy * ticker.CurrentPrice;
                            customer.TradingAccount.DebitBalance(buyOperationValue);
                            customer.TradingAccount.AddCustody(ticker.Symbol, quantityToBuy, ticker.CurrentPrice);
                            await _taxService.PostTaxInKafkaAsync(customer,ticker.Symbol,buyOperationValue,_taxService.CalculateRetentionTax(buyOperationValue),TaxType.DedoDuro);
                    }
                }
            }
            masterAccount.CreditBalance(customer.TradingAccount.Balance);
            customer.TradingAccount.DebitBalance(customer.TradingAccount.Balance);
        }

        await _customerRepository.SaveChangesAsync();
        return true;
    }
}
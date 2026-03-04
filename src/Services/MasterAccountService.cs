using Repositories.Interfaces;

public class MasterAccountService : IMasterAccountService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ITickerRepository _tickerRepository;

    public MasterAccountService(ICustomerRepository customerRepository, ITickerRepository tickerRepository)
    {
        _customerRepository = customerRepository;
        _tickerRepository = tickerRepository;
    }
    public async Task<MasterCustodyResponse> GetMasterCustodyAsync()
    {
        var masterAccount = await _customerRepository.GetCustomerWithPortfolioAsync(1);
        
        if (masterAccount == null)
            throw new CustomException("CONTA_MASTER_NAO_ENCONTRADA");

        var symbols = masterAccount.Custodies.Select(c => c.Symbol).ToList();
        var currentPrices = await _tickerRepository.GetTickersDictBySymbol(symbols);

        var custodia = masterAccount.Custodies.Select(c => {
            var precoAtual = currentPrices.GetValueOrDefault(c.Symbol)?.CurrentPrice ?? c.AveragePrice;
            return new MasterAssetDto(
                Ticker: c.Symbol,
                Quantidade: c.Quantity,
                PrecoMedio: c.AveragePrice,
                ValorAtual: precoAtual,
                //Origem: c.OriginNote ?? "Resíduo de distribuição",
                Origem: "Teste"
            );
        }).ToList();
        string numeroContaMaster = $"MST-{masterAccount.TradingAccount.Id.ToString().PadLeft(6, '0')}";
        return new MasterCustodyResponse(
            new MasterAccountDto(masterAccount.Id, numeroContaMaster, "MASTER"),
            custodia,
            custodia.Sum(x => x.Quantidade * x.ValorAtual)
        );
    }
}


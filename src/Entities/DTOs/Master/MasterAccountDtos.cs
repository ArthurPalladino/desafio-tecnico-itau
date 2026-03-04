public record MasterCustodyResponse(
    MasterAccountDto ContaMaster,
    List<MasterAssetDto> Custodia,
    decimal ValorTotalResiduo
);

public record MasterAccountDto(int Id, string NumeroConta, string Tipo);

public record MasterAssetDto(
    string Ticker, 
    int Quantidade, 
    decimal PrecoMedio, 
    decimal ValorAtual, 
    string Origem
);
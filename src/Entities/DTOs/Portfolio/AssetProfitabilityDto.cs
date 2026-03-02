public record PortfolioProfitabilityResponse(
    string CustomerName,
    decimal TotalInvested,      // Soma de (Qtde * Preço Médio)
    decimal CurrentTotalValue,   // Soma de (Qtde * Preço Atual)
    decimal TotalProfitLoss,    // Diferença em R$
    decimal ProfitPercentage,    // Diferença em %
    List<AssetProfitabilityDto> Assets
);

public record AssetProfitabilityDto(
    string Symbol,
    int Quantity,
    decimal AveragePrice,
    decimal CurrentPrice,
    decimal ProfitLoss,
    decimal VariationPercentage
);
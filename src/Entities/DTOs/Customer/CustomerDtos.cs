
// --- REQUESTS (O que a API recebe) ---
public record CreateCustomerRequest(string Nome, string Cpf, string Email, decimal ValorMensal);

// --- RESPONSES (O que a API devolve) ---

public record CustomerResponse(
    int ClienteId,
    string Nome,
    string Cpf,
    string Email,
    decimal ValorMensal,
    bool Ativo,
    DateTime DataAdesao,
    GraphicAccountResponse ContaGrafica
);

public record GraphicAccountResponse(
    int Id,
    string NumeroConta,
    string Tipo,
    DateTime DataCriacao
);

public record SubscriptionChangeResponse(
    int ClienteId,
    string Nome,
    bool Ativo,
    DateTime? DataSaida,
    string Mensagem
);

public record UpdateAmountResponse(
    int ClienteId,
    decimal ValorMensalAnterior,
    decimal ValorMensalNovo,
    DateTime DataAlteracao,
    string Mensagem
);

public record PortfolioSummaryResponse(
    int ClienteId, 
    string Nome, 
    string ContaGrafica, 
    DateTime DataConsulta, 
    PortfolioMetrics Resumo, 
    List<AssetDto> Ativos
);

public record PortfolioMetrics(
    decimal ValorTotalInvestido, 
    decimal ValorAtualCarteira, 
    decimal PlTotal, 
    decimal RentabilidadePercentual
);

public record AssetDto(
    string Ticker, 
    int Quantidade, 
    decimal PrecoMedio, 
    decimal CotacaoAtual, 
    decimal ValorAtual, 
    decimal Pl, 
    decimal PlPercentual, 
    decimal ComposicaoCarteira
);

public record PortfolioProfitabilityResponse(
    int ClienteId, 
    string Nome, 
    DateTime DataConsulta, 
    PortfolioMetrics Rentabilidade, 
    List<AporteDto> HistoricoAportes, 
    List<EvolucaoDto> EvolucaoCarteira
);

public record AporteDto(string Data, decimal Valor, string Parcela);
public record EvolucaoDto(string Data, decimal ValorCarteira, decimal ValorInvestido, decimal Rentabilidade);
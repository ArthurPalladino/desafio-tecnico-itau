public class MotorCompraResponseDto
{
    public DateTime DataExecucao { get; set; }
    public int TotalClientes { get; set; }
    public decimal TotalConsolidado { get; set; }
    public List<OrdemResumoDto> OrdensCompra { get; set; } = new();
    public List<DistribuicaoAgrupadaDto> Distribuicoes { get; set; } = new();
    public List<ResiduoMasterDto> ResiduosCustMaster { get; set; } = new();
    public int EventosIRPublicados { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}

public class OrdemResumoDto
{
    public string Ticker { get; set; } = string.Empty;
    public int QuantidadeTotal { get; set; }
    public List<OrdemDetalheDto> Detalhes { get; set; } = new();
    public decimal PrecoUnitario { get; set; }
    public decimal ValorTotal { get; set; }
}

public class OrdemDetalheDto
{
    public string Tipo { get; set; } = string.Empty;
    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class DistribuicaoAgrupadaDto
{
    public int ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal ValorAporte { get; set; }
    public List<AtivoDistribuidoDto> Ativos { get; set; } = new();
}

public class AtivoDistribuidoDto
{
    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class ResiduoMasterDto
{
    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}
public class CreateBasketRequest
{
    public string nome { get; set; }
    public List<BasketItemRequest> itens { get; set; }
}

public class BasketItemRequest
{
    public string ticker { get; set; }
    public decimal percentual { get; set; }
}

public class CreateBasketResponse
{
    public int cestaId { get; set; }
    public string nome { get; set; }
    public bool ativa { get; set; }
    public DateTime dataCriacao { get; set; }
    public List<CreateBasketItemResponse> itens { get; set; }
    public bool rebalanceamentoDisparado { get; set; }
    public string mensagem { get; set; }
    public CestaAnteriorInfo cestaAnteriorDesativada { get; set; }
    public List<string> ativosRemovidos { get; set; }
    public List<string> ativosAdicionados { get; set; }
}

public class BasketHistoryResponse
{
    public List<BasketSummaryResponse> cestas { get; set; }
}
public class BasketSummaryResponse
{
    public int cestaId { get; set; }
    public string nome { get; set; }
    public bool ativa { get; set; }
    public DateTime dataCriacao { get; set; }
    public DateTime? dataDesativacao { get; set; } 
    public List<CreateBasketItemResponse> itens { get; set; }
}


public class CreateBasketItemResponse
{
    public string ticker { get; set; }
    public decimal percentual { get; set; }
}

public class BasketAtualResponse
{
    public int cestaId { get; set; }
    public string nome { get; set; }
    public bool ativa { get; set; }
    public DateTime dataCriacao { get; set; }
    public List<BasketItemAtualResponse> itens { get; set; }
}

public class BasketItemAtualResponse
{
    public string ticker { get; set; }
    public decimal percentual { get; set; }
    public decimal? cotacaoAtual { get; set; } = null;
}

public class CestaAnteriorInfo
{
    public int cestaId { get; set; }
    public string nome { get; set; }
    public DateTime dataDesativacao { get; set; }
}


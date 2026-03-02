public record ErrorDetail(int StatusCode, string Message);

public static class ErrorCatalog
{
    private static readonly Dictionary<string, ErrorDetail> _errors = new()
    {
        // --- 400 BAD REQUEST (Regras Oficiais do Contrato) ---
        { "CLIENTE_CPF_DUPLICADO", new(400, "CPF ja cadastrado no sistema.") },
        { "VALOR_MENSAL_INVALIDO", new(400, "O valor mensal minimo e de R$ 100,00.") },
        { "PERCENTUAIS_INVALIDOS", new(400, "A soma dos percentuais deve ser exatamente 100%.") },
        { "QUANTIDADE_ATIVOS_INVALIDA", new(400, "A cesta nao contem exatamente 5 ativos.") },
        { "CLIENTE_JA_INATIVO", new(400, "Cliente ja havia saido do produto.") },

        // --- 400 BAD REQUEST (Validações Específicas) ---
        { "NOME_OBRIGATORIO", new(400, "O nome é obrigatório.") },
        { "CPF_INVALIDO", new(400, "CPF inválido. O campo deve conter exatamente 11 dígitos numéricos.") },
        { "TICKER_OBRIGATORIO", new(400, "O símbolo do ativo (ticker) é obrigatório.") },
        { "ID_CLIENTE_INVALIDO", new(400, "O ID do cliente é inválido.") },
        { "ID_CONTA_INVALIDO", new(400, "O ID da conta gráfica é inválido.") },
        { "ORDEM_COMPRA_INVALIDA", new(400, "A ordem de compra informada é inválida.") },
        { "CONTA_FILHOTE_INVALIDA", new(400, "A conta filhote informada é inválida.") },
        { "CONTA_MASTER_INVALIDA", new(400, "A conta master informada é inválida.") },
        { "QUANTIDADE_NEGATIVA", new(400, "A quantidade não pode ser negativa.") },
        { "PRECO_NEGATIVO", new(400, "O preço não pode ser negativo.") },
        { "CUSTODIA_INSUFICIENTE", new(400, "Quantidade insuficiente em custódia.") },
        { "PERCENTUAL_INVALIDO", new(400, "O percentual deve ser maior que 0 e menor ou igual a 100.") },
        { "IMPOSTO_INVALIDO", new(400, "O valor do imposto não pode ser negativo.") },

        // --- 404 NOT FOUND ---
        { "CLIENTE_NAO_ENCONTRADO", new(404, "Cliente nao encontrado.") },
        { "CESTA_NAO_ENCONTRADA", new(404, "Nenhuma cesta ativa encontrada.") },
        { "COTACAO_NAO_ENCONTRADA", new(404, "Arquivo COTAHIST nao encontrado para a data.") },

        // --- 409 CONFLICT / 500 SERVER ---
        { "COMPRA_JA_EXECUTADA", new(409, "Compra ja foi executada para esta data.") },
        { "KAFKA_INDISPONIVEL", new(500, "Erro ao publicar no topico Kafka.") }
    };

    public static ErrorDetail Get(string code) 
        => _errors.TryGetValue(code, out var detail) ? detail : new(500, "Erro interno desconhecido.");
}
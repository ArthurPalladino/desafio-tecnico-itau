using Repositories.Interfaces;

public class TaxService : ITaxService
{
    private readonly ITaxEventRepository _taxEventRepository;
    private readonly IKafkaProducer _kafkaProducer;
    private const decimal _retentionTaxRate = 0.00005m;

    private const decimal _capitalGainTaxRate = 0.20m;

    private const decimal _taxExemptionLimit = 20000.00m;

    decimal ITaxService.RetentionTaxRate => _retentionTaxRate;

    public TaxService(ITaxEventRepository taxEventRepository, IKafkaProducer kafkaProducer)
    {
        _kafkaProducer = kafkaProducer;
        _taxEventRepository = taxEventRepository;
    }
    public async Task PostTaxInKafkaAsync(Customer customer, string symbol, decimal operationValue, decimal taxValue, TaxType taxType)
    {
        if (taxValue<=0) return;
        TaxEvent taxEvent = new TaxEvent(customer.Id, operationValue, taxValue, taxType);
        await _taxEventRepository.AddAsync(taxEvent);

        string topic = taxType == TaxType.DedoDuro ? "ir-dedo-duro" : "ir-venda";
        
        var kafkaMessage = new
        {
            tipo = taxType.ToString().ToUpper(),
            clienteId = customer.Id,
            cpf = customer.Cpf,
            ticker = symbol,
            valorOperacao = operationValue,
            valorIR = taxValue,
            dataOperacao = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };

        await _kafkaProducer.ProduceAsync(topic, customer.Id.ToString(), kafkaMessage);
        
        taxEvent.PublicInKafka(); 
    }


    public decimal CalculateRetentionTax(decimal operationValue)
    {
        return Math.Round(operationValue * _retentionTaxRate, 2); 
    }
    public decimal CalculateCapitalGainTax(decimal saleValue, decimal averagePrice, int quantity, decimal totalSalesInMonth)
    {
        if (totalSalesInMonth <= _taxExemptionLimit)
        {
            return 0m;
        }

        decimal totalCost = quantity * averagePrice;
        decimal profit = saleValue - totalCost;

        if (profit <= 0)
        {
            return 0m;
        }

        return Math.Round(profit * _capitalGainTaxRate, 2); 
    }
}
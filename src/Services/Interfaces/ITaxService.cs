public interface ITaxService
{
    decimal RetentionTaxRate { get; }
    decimal CalculateRetentionTax(decimal operationValue);
    decimal CalculateCapitalGainTax(decimal saleValue, decimal averagePrice, int quantity, decimal totalSalesInMonth);
    Task PostTaxInKafkaAsync(Customer customer, string symbol, decimal operationValue, decimal taxValue, TaxType taxType);
}
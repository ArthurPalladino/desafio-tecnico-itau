using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Eventing.Reader;

public class TaxEvent
{
    public int Id { get; private set; }

    public int CustomerId { get; private set; }

    public decimal BaseAmount { get; private set; }

    public decimal TaxAmount { get; private set; }

    public DateTime EventDate { get; private set; }

    public TaxType Type { get; private set; }

    public bool AlreadyInKafka {get; private set;}
    public Customer? Customer { get; private set; }

    // EF
    protected TaxEvent() { }

    public TaxEvent(int customerId, decimal baseAmount, decimal taxAmount, TaxType type)
    {
       if (customerId <= 0) 
            throw new CustomException("ID_CLIENTE_INVALIDO");

        if (baseAmount < 0) 
            throw new CustomException("VALOR_NEGATIVO");

        if (taxAmount < 0) 
            throw new CustomException("IMPOSTO_INVALIDO");

        CustomerId = customerId;
        BaseAmount = baseAmount;
        TaxAmount = taxAmount;
        Type = type;
        EventDate = DateTime.Now;
    }
}
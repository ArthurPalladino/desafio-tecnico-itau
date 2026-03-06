using Xunit;
using Moq;
using FluentAssertions;
using Repositories.Interfaces;
using System.Threading.Tasks;
using System;

namespace Tests.Services.Tests
{
    public class TaxServiceTests
    {
        private readonly Mock<ITaxEventRepository> _taxEventRepoMock;
        private readonly Mock<IKafkaProducer> _kafkaProducerMock;
        private readonly TaxService _service;

        public TaxServiceTests()
        {
            _taxEventRepoMock = new Mock<ITaxEventRepository>();
            _kafkaProducerMock = new Mock<IKafkaProducer>();
            _service = new TaxService(_taxEventRepoMock.Object, _kafkaProducerMock.Object);
        }

        [Fact]
        public void CalculateRetentionTax_ShouldReturnCorrectValue_WhenStandardTransaction()
        {
            var result = _service.CalculateRetentionTax(100000m);
            
            result.Should().Be(5.00m);
        }

        [Fact]
        public void CalculateRetentionTax_ShouldReturnZero_WhenZeroValue()
        {
            var result = _service.CalculateRetentionTax(0);
            
            result.Should().Be(0);
        }

        [Fact]
        public void CalculateCapitalGainTax_ShouldReturnZero_WhenBelowExemptionLimit()
        {
            var result = _service.CalculateCapitalGainTax(25000m, 20m, 100, 1000m);
            
            result.Should().Be(0);
        }

        [Fact]
        public void CalculateCapitalGainTax_ShouldCalculateCorrectly_WhenAboveExemptionLimit()
        {
            var result = _service.CalculateCapitalGainTax(30000m, 100m, 100, 25000m);

            result.Should().Be(4000m);
        }

        [Fact]
        public void CalculateCapitalGainTax_ShouldReturnZero_WhenNoProfit()
        {
            var result = _service.CalculateCapitalGainTax(20000m, 100m, 100, 19000m);
            
            result.Should().Be(0);
        }

        [Fact]
        public async Task PostTaxInKafkaAsync_ShouldPublishEvent_WhenValid()
        {
            var customer = new Customer("Test", "12345678909", "test@example.com", 300);
            customer.GetType().GetProperty("Id")?.SetValue(customer, 1);

            _taxEventRepoMock.Setup(x => x.AddAsync(It.IsAny<TaxEvent>())).Returns(Task.CompletedTask);
            _kafkaProducerMock.Setup(x => x.ProduceAsync<object>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns(Task.CompletedTask);

            await _service.PostTaxInKafkaAsync(customer, "PETR4", 10000m, 5m, TaxType.DedoDuro);

            _taxEventRepoMock.Verify(x => x.AddAsync(It.IsAny<TaxEvent>()), Times.Once);
            _kafkaProducerMock.Verify(x => x.ProduceAsync<object>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task PostTaxInKafkaAsync_ShouldNotPublish_WhenTaxIsZero()
        {
            var customer = new Customer("Test", "12345678909", "test@example.com", 300);
            customer.GetType().GetProperty("Id")?.SetValue(customer, 1);

            await _service.PostTaxInKafkaAsync(customer, "PETR4", 10000m, 0, TaxType.DedoDuro);

            _taxEventRepoMock.Verify(x => x.AddAsync(It.IsAny<TaxEvent>()), Times.Never);
            _kafkaProducerMock.Verify(x => x.ProduceAsync<object>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never);
        }
    }
}

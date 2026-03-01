using Confluent.Kafka;
using System.Text.Json;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(IConfiguration configuration)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] 
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task ProduceAsync<T>(string topic, string key, T message)
    {
        var value = JsonSerializer.Serialize(message);
        var kafkaMessage = new Message<string, string> { Key = key, Value = value };
        
        await _producer.ProduceAsync(topic, kafkaMessage);
    }
}
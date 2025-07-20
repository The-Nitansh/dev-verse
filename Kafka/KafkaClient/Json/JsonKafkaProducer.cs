using KafkaClient.Configuration;
using KafkaClient.Interfaces;
using Microsoft.Extensions.Logging;

namespace KafkaClient.Json;

public class JsonKafkaProducer(ILogger<JsonKafkaProducer> logger, KafkaConfig config) : IKafkaProducer<string>
{
    public async Task ProduceAsync(string message)
    {
        // Produce JSON message to Kafka
    }
}
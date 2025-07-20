using KafkaClient.Configuration;
using KafkaClient.Interfaces;


namespace KafkaClient.Json;

public class JsonKafkaConsumer : IKafkaConsumer
{
    private readonly KafkaConfig _config;

    public JsonKafkaConsumer(KafkaConfig config)
    {
        _config = config;
        // Initialize Kafka consumer instance here using _config.BootstrapServers
    }

    public void StartConsuming(Func<string, Task> messageHandler)
    {
        // Start listening to _config.Topic and invoke messageHandler for each message
    }
}
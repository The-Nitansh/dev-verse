using Confluent.Kafka;
using KafkaProducerApp.DataModels;
using KafkaProducerApp.Interfaces;
using Microsoft.Extensions.Options;

namespace KafkaProducerApp.Services;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly ILogger<KafkaProducerService> _logger;
    private readonly string _topic;
    private readonly IProducer<Null, string> _producer;
    private readonly Dictionary<string, List<int>> _utilityPartitions;
    private readonly Dictionary<string, int> _utilityPartitionCursor;
    private readonly KafkaSettings _kafkaSettings;

    public KafkaProducerService(
        ILogger<KafkaProducerService> logger,
        IOptions<KafkaSettings> kafkaOptions)
    {
        _logger = logger;
        _kafkaSettings = kafkaOptions.Value;
        _topic = _kafkaSettings.Topic;

        _producer = new ProducerBuilder<Null, string>(new ProducerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers
        }).Build();

        _utilityPartitions = _kafkaSettings.Utilities.ToDictionary(
            u => u.Name,
            u => u.Partitions
        );

        _utilityPartitionCursor = _kafkaSettings.Utilities.ToDictionary(
            u => u.Name,
            u => 0
        );

        _logger.LogInformation("Kafka Producer initialized with Topic: {Topic} | TotalPartitions: {PartitionCount}", _topic, _kafkaSettings.TotalPartitions);
    }

    public async Task ProduceMessageAsync(string utilityName, string message)
    {
        if (!_utilityPartitions.TryGetValue(utilityName, out var partitions) || partitions.Count == 0)
        {
            _logger.LogWarning("Utility '{UtilityName}' not found or has no assigned partitions. Skipping message.", utilityName);
            return;
        }

        var currentCursor = _utilityPartitionCursor[utilityName];
        var targetPartition = new Partition(partitions[currentCursor]);

        _utilityPartitionCursor[utilityName] = (currentCursor + 1) % partitions.Count;

        try
        {
            var deliveryResult = await _producer.ProduceAsync(
                new TopicPartition(_topic, targetPartition),
                new Message<Null, string> { Value = message }
            );

            Thread.Sleep(400);

            _logger.LogInformation("Message sent to {Topic} | Utility: {Utility} | Partition: {Partition} | Offset: {Offset}",
                _topic, utilityName, deliveryResult.Partition.Value, deliveryResult.Offset.Value);
        }
        catch (ProduceException<Null, string> ex)
        {
            _logger.LogError(ex, "Error producing Kafka message for utility {Utility}: {Reason}", utilityName, ex.Error.Reason);
        }
    }
}

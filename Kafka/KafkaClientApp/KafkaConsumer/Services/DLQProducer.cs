using Confluent.Kafka;
using KafkaConsumer.DataModels;
using KafkaConsumer.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace KafkaConsumer.Services;

public class DLQProducer : IDLQProducer
{
    private readonly IProducer<Null, string> producer;
    private readonly string topic;
    private readonly ILogger<DLQProducer> logger;

    public DLQProducer(ILogger<DLQProducer> logger, IOptions<KafkaSettings> kafkaSettings)
    {
        this.logger = logger;
        topic = kafkaSettings.Value.DlqTopic;

        var config = new ProducerConfig
        {
            BootstrapServers = kafkaSettings.Value.BootstrapServers,
            Acks = Acks.All
        };

        producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task SendToDlqAsync(DLQMessage message, CancellationToken cancellationToken = default)
    {
        var payload = JsonConvert.SerializeObject(message);

        try
        {
            var deliveryResult = await producer.ProduceAsync(topic, new Message<Null, string>{ Value = payload }, cancellationToken);
            logger.LogWarning("DLQ: Message sent to DLQ topic {topic} at offset {offset}", topic, deliveryResult.Offset);
        }
        catch (Exception ex) { logger.LogError(ex, "DLQ: Failed to send message to DLQ topic {topic}", topic); }
    }
}



using Confluent.Kafka;
using KafkaClient.Interfaces;
using Microsoft.Extensions.Logging;


namespace KafkaClient.Avro;

public class AvroKafkaConsumer<T> : IKafkaConsumer<T>
{

    private readonly ILogger<AvroKafkaConsumer<T>> logger;
    private KafkaConsumerConfiguration ConsumerConfig { get; set; }

    private IConsumer<string, string> Consumer { get; set; }
    private Func<string, T> Handler { get; init; }

    public AvroKafkaConsumer(ILogger<AvroKafkaConsumer<T>> logger, KafkaConsumerConfiguration consumerConfig, string consumerId, Func<string, T> handler)
    {
        ConsumerConfig = consumerConfig;
        ConsumerConfig.ClientId = consumerId;
        //ConsumerId = consumerId;
        Handler = handler;
        this.logger = logger;

        InitConsumer();
    }

    private void InitConsumer()
    {
        Consumer = new ConsumerBuilder<string, string>(QConfiguration.GetConsumerConfig(ConsumerConfig))
                    .SetErrorHandler((_, e) => logger.LogError(e.ToString()))
                    .Build();
    }

    public T StartConsuming(CancellationToken cancellationToken)
    {
        T result = default;

        Consumer.Subscribe(ConsumerConfig.Topic);
        logger.LogInformation($"Subscribed to topic : {ConsumerConfig.Topic}");

        while (!cancellationToken.IsCancellationRequested)
        {
            var consumerResult = Consumer.Consume(cancellationToken);

            if (consumerResult?.Message?.Value != null)
            {
                result = Handler(consumerResult.Message.Value);
            }
        }
        Consumer.Close();

        return result;
    }
}
using Confluent.Kafka;
using KafkaClient.Interfaces;
using Microsoft.Extensions.Logging;


namespace KafkaClient.Json;

public class JsonKafkaConsumer<T> : IKafkaConsumer<T>
{

    private readonly ILogger<JsonKafkaConsumer<T>> logger;
    private string ConsumerId { get; init; }
    private KafkaConsumerConfiguration ConsumerConfig { get; set; }
    
    private IConsumer<string, string> Consumer { get; set; }
    private Func<ConsumeResult<string, string>, T> Handler {  get; init; }

    public JsonKafkaConsumer(ILogger<JsonKafkaConsumer<T>> logger, KafkaConsumerConfiguration consumerConfigOptions, string consumerId, Func<ConsumeResult<string, string>, T> handler)
    {
        ConsumerConfig = consumerConfigOptions;
        ConsumerId = consumerId;
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
            var consumeResult = Consumer.Consume(cancellationToken);
            result = Handler(consumeResult);
        }
        Consumer.Close();

        return result;
    }
}
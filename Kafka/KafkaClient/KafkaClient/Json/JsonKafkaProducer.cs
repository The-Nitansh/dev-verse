using Confluent.Kafka;
using KafkaClient.Interfaces;
using Microsoft.Extensions.Logging;
using static Confluent.Kafka.ConfigPropertyNames;

namespace KafkaClient.Json;

public class JsonKafkaProducer<T> : IKafkaProducer<T>
{
    private readonly ILogger<JsonKafkaProducer<T>> logger;
    private IProducer<string, string> Producer {  get; set; }
    private KafkaProducerConfiguration ProducerConfig { get; set; }

    public JsonKafkaProducer(ILogger<JsonKafkaProducer<T>> logger, KafkaProducerConfiguration producerConfig)
    {
        this.logger = logger;
        ProducerConfig = producerConfig;

        InitProducer();
    }

    private void InitProducer()
    {
        Producer = new ProducerBuilder<string, string>(QConfiguration.GetProducerConfig(ProducerConfig))
                    .SetErrorHandler((_, e) => logger.LogError(e.ToString()))
                    .Build();
    }

    public async Task ProduceMessageAsync(string topic, string key, string value)
    {
        try
        {
            var result = await Producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = key,
                Value = value
            });

            // Optional: log success or metrics.
        }
        catch (ProduceException<string, string> ex)
        {
            /*OnProduceErrorOccur(new QueueProduceErrorEventArgs()
            {
                Topic = topic,
                Key = key,
                Value = value,
                FailedOperation = "ProduceMessage",
                Message = ex.Message,
                StackTrace = ex.StackTrace
            });*/
        }
        catch (Exception ex)
        {
            /*OnProduceErrorOccur(new QueueProduceErrorEventArgs()
            {
                Topic = topic,
                Key = key,
                Value = value,
                FailedOperation = "ProduceMessage",
                Message = ex.Message,
                StackTrace = ex.StackTrace
            });*/
        }
    }

}


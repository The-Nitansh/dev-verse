using Confluent.Kafka;
using KafkaClient.Avro;
using KafkaClient.Interfaces;
using KafkaClient.Json;
using Microsoft.Extensions.Logging;

namespace KafkaClient.Factory;



public static class KafkaClientFactory<T, R>
{
    public static JsonKafkaConsumer<T> BuildJsonConsumer(ILogger<JsonKafkaConsumer<T>> logger, KafkaConsumerConfiguration config, string consumerId, Func<ConsumeResult<string, string>, T> handler)
    {
        return new JsonKafkaConsumer<T>(logger, config, consumerId, handler);
    }

    public static AvroKafkaConsumer<T> BuildAvroConsumer(ILogger<AvroKafkaConsumer<T>> logger, KafkaConsumerConfiguration config, string consumerId, Func<string, T> handler)
    {
        return new AvroKafkaConsumer<T>(logger, config, consumerId, handler);
    }

    public static IKafkaProducer<R> BuildJsonProducer(KafkaProducerConfiguration config)
    {
        throw new NotImplementedException("Producer not yet implemented.");
    }

    public static IKafkaProducer<R> BuildAvroProducer(KafkaProducerConfiguration config)
    {
        throw new NotImplementedException("Producer not yet implemented.");
    }
}



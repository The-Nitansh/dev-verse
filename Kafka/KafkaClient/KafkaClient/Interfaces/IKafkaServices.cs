namespace KafkaClient.Interfaces;

public interface IKafkaProducer<T>
{
    Task ProduceMessageAsync(string topic, string key, string value);
}

public interface IKafkaConsumer<T>
{
    T StartConsuming(CancellationToken cancellationToken);
}
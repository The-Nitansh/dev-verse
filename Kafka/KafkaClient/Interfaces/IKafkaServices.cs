namespace KafkaClient.Interfaces;

public class IKafkaServices
{
    
}

public interface IKafkaProducer<TMessage>
{
    Task ProduceAsync(TMessage message);
}

public interface IKafkaConsumer
{
    void StartConsuming(Func<string, Task> messageHandler);
}
using KafkaConsumer.DataModels;
using KafkaConsumer.Interfaces;

namespace KafkaConsumer.Services;

public class ConsumerCoordinator(ILogger<ConsumerCoordinator> logger, KafkaConsumerHandlerFactory consumerHandlerFactory)
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        for (int consumerId = 0; consumerId < Global.ConsumerCount; consumerId++)
        {
            var handler = consumerHandlerFactory(consumerId);
            logger.LogInformation("Starting Kafka consumer with ID: {consumerId}", consumerId);
            _ = Task.Run(() => handler.StartConsumingAsync(cancellationToken), cancellationToken);
        }
    }
}

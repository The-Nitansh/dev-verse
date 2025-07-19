using KafkaConsumer.Services;
using System.Collections.Generic;

namespace KafkaConsumer;

public class Worker(ILogger<Worker> logger, ConsumerCoordinator consumerCoordinator) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
		try
		{
			await consumerCoordinator.StartAsync(stoppingToken);
		}
		catch (Exception ex)
		{
            logger.LogError(ex, "Kafka Consumer Worker encountered a fatal error.");
        }
    }
}
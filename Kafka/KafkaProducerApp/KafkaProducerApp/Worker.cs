using KafkaProducer.Utilities;
using KafkaProducerApp.Services;

namespace KafkaProducerApp
{
    public class Worker(ILogger<Worker> logger, KafkaProducerService kafkaProducerService) : BackgroundService
    {
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);

            int aId, bId, cId;
            aId = bId = cId = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                
                try
                {
                    await kafkaProducerService.ProduceMessageAsync("UtilityA", JsonHelper.GetSerializedObject(new DataModels.DummyModel{ Id=aId++, Description="Sample message for Utility A"}));
                    //await kafkaProducerService.ProduceMessageAsync("UtilityA", JsonHelper.GetSerializedObject(new DataModels.DummyModel{ Id=aId++, Description="Sample message for Utility A"}));
                    //await kafkaProducerService.ProduceMessageAsync("UtilityB", JsonHelper.GetSerializedObject(new DataModels.DummyModel{ Id=aId++, Description="Sample message for Utility A"}));
                    //await kafkaProducerService.ProduceMessageAsync("UtilityB", JsonHelper.GetSerializedObject(new DataModels.DummyModel{ Id=aId++, Description="Sample message for Utility A"}));
                    //await kafkaProducerService.ProduceMessageAsync("UtilityB", JsonHelper.GetSerializedObject(new DataModels.DummyModel{ Id=bId++, Description="Sample message for Utility B"}));
                    //await kafkaProducerService.ProduceMessageAsync("UtilityB", JsonHelper.GetSerializedObject(new DataModels.DummyModel{ Id=bId++, Description="Sample message for Utility B"}));
                    //await kafkaProducerService.ProduceMessageAsync("UtilityB", JsonHelper.GetSerializedObject(new DataModels.DummyModel{ Id=cId++, Description="Sample message for Utility C"}));
                    //await kafkaProducerService.ProduceMessageAsync("UtilityB", JsonHelper.GetSerializedObject(new DataModels.DummyModel{ Id=cId++, Description="Sample message for Utility C"}));
                    //await kafkaProducerService.ProduceMessageAsync("UtilityB", JsonHelper.GetSerializedObject(new DataModels.DummyModel{ Id=cId++, Description="Sample message for Utility C"}));

                    await Task.Delay(3000, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while producing messages.");
                }
            }
            logger.LogInformation("Worker stopped at: {time}", DateTimeOffset.Now);
        }
    }
}

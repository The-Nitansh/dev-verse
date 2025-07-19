using KafkaConsumer.DataModels;
using KafkaConsumer.Interfaces;

namespace KafkaConsumer.Services;

class DummyService(ILogger<DummyService> logger) : IDummyService
{
    public ReturnCode DummyProcessor(DummyModel dummyInput, out DummyOutput dummyOutput)
    {
        var sleepTime = new Random().Next(4000, 4500)/1000;
        Thread.Sleep(sleepTime);
        switch (new Random().Next(1, 101))
        {
            case >= 95 :
                dummyOutput = new DummyOutput
                {
                    ConsumerId = dummyInput.Id,
                    Result = "Failed",
                    Error = "Data Processig Error"
                };
                return ReturnCode.Failure;

            case >= 80 and < 95:
                dummyOutput = new DummyOutput
                {
                    ConsumerId = dummyInput.Id,
                    Result = "Corrupt payload",
                    Error = "Payload is corrupted"
                };
                return ReturnCode.Failure;

            default:
                dummyOutput = new DummyOutput
                {
                    ConsumerId = dummyInput.Id,
                    Result = "Request Processed successfully",
                    Error = string.Empty
                };
                return ReturnCode.Success;
        }
    }
}

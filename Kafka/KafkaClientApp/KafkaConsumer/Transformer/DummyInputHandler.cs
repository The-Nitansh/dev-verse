using Confluent.Kafka;
using KafkaConsumer.DataModels;
using KafkaConsumer.Utilities;
using static System.String;

namespace KafkaConsumer.Transformer;

public class DummyInputHandler(Action<string> logAction)
{
    public ReturnCode CreateAndValidateDummyModelAsync(string message, CancellationToken cancellationToken, out DummyModel? output)
    {
        output = null;
        var jsonResult = JsonHelper.GetObject<DummyModel>(message).OnError(ex => { logAction.Invoke(ex.Message); });
        if ( !jsonResult.IsSuccess ) { return ReturnCode.Failure; }
        return ValidateDummyModel(output = jsonResult.Value);

    }

    private ReturnCode ValidateDummyModel(DummyModel dummyModel)
    {
        if (dummyModel == null) { return ReturnCode.Failure; }
        if (IsNullOrWhiteSpace(dummyModel.Description)) { return ReturnCode.Failure; }
        return ReturnCode.Success;
    }
}

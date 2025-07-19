using Newtonsoft.Json;

namespace KafkaConsumer.Utilities;

public class JsonResult<T>
{
    public T? Value { get; private set; }
    public Exception? Exception { get; private set; }
    public bool IsSuccess => Exception == null;

    public JsonResult(T? value) => Value = value;
    public JsonResult(Exception ex) => Exception = ex;

    public JsonResult<T> OnError(Action<Exception> handler)
    {
        if (Exception != null) { handler(Exception); }
        return this;
    }
}

public static class JsonHelper
{
    public static JsonResult<T> GetObject<T>(string json)
    {
        try { return new JsonResult<T>(JsonConvert.DeserializeObject<T>(json)); }
        catch (Exception ex) { return new JsonResult<T>(ex); }
    }
}

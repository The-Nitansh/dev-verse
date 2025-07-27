namespace KafkaApp.Logging;

public static class CustomLoggerFactory
{
    private static ILoggerFactory loggerFactory;

    public static void Configure(ILoggerFactory factory)
    {
        CustomLoggerFactory.loggerFactory = factory;
    }

    public static ILogger<T> For<T>()
    {
        if (loggerFactory == null)
            throw new InvalidOperationException("CustomLoggerFactory not configured. Call Log.Configure() first.");

        return loggerFactory.CreateLogger<T>();
    }
}

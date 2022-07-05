using Microsoft.Extensions.Hosting;

namespace Toolkit.Mongo;

public static class MongoEnvironment
{
    private static string _StringConnection;
    private static string _DataBaseName;
    public static string StringConnection
        => _StringConnection;
    public static string DataBaseName
        => _DataBaseName;
    public static IHostBuilder AddMongoDb(this IHostBuilder builder,
        string stringConnection, string databaseName)
    {
        if (builder == null)
            throw new ArgumentNullException("Builder not provided. Unable to start Mongo Environment.");
        if (stringConnection.IsEmpty())
            throw new ArgumentNullException("Mongo Connection not provided. Unable to start Mongo Environment.");
        _StringConnection = Environment.GetEnvironmentVariable(stringConnection);
        if (_StringConnection.IsEmpty())
            throw new ArgumentNullException($"Unable to identify {_StringConnection} variable. Unable to start Mongo Environment.");
        _DataBaseName = Environment.GetEnvironmentVariable(databaseName);
        if (_DataBaseName.IsEmpty())
            throw new ArgumentNullException($"Unable to identify {_DataBaseName} variable. Unable to start Mongo Environment.");
        return builder;
    }
}
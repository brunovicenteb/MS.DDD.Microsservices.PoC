using Microsoft.Extensions.DependencyInjection;

namespace Toolkit.Mongo;

public static class MongoEnvironment
{
    private static string _StringConnection;
    private static string _DataBaseName;
    public static string StringConnection
        => _StringConnection;
    public static string DataBaseName
        => _DataBaseName;

    public static IServiceCollection AddMongoDb(this IServiceCollection services,
        string stringConnection = "MONGODB_URL", string databaseName = "MONGODB_DATABASE")
    {
        if (services == null)
            throw new ArgumentNullException("Services Collection not provided. Unable to start Mongo Environment.");
        if (stringConnection.IsEmpty())
            throw new ArgumentNullException("Mongo Connection not provided. Unable to start Mongo Environment.");
        _StringConnection = Environment.GetEnvironmentVariable(stringConnection);
        if (_StringConnection.IsEmpty())
            throw new ArgumentNullException($"Unable to identify {_StringConnection} variable. Unable to start Mongo Environment.");
        _DataBaseName = Environment.GetEnvironmentVariable(databaseName);
        if (_DataBaseName.IsEmpty())
            throw new ArgumentNullException($"Unable to identify {_DataBaseName} variable. Unable to start Mongo Environment.");
        return services;
    }
}
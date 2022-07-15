using Toolkit.OutBox.Consumer;
using Toolkit.OutBox.Producer;
using Toolkit.TransactionalOutBox;
using Microsoft.AspNetCore.Builder;

namespace Toolkit.OutBox;

public static class OutBoxFactory
{
    public static ILogable BeginProducer<T>(this WebApplicationBuilder builder, DatabaseType dbType,
        bool recreateDb = false, string dbConnectionVarName = "DATABASE_CONNECTION") where T : OutBoxDbContext
    {
        return new ProducerOutBoxStarter<T>(builder, dbType, recreateDb, dbConnectionVarName);
    }

    public static ILogable BeginConsumer<T>(this WebApplicationBuilder builder, DatabaseType dbType,
        string dbConnectionVarName = "DATABASE_CONNECTION") where T : OutBoxDbContext, new()
    {
        return new ConsumerOutBoxStarter<T>(builder, dbType, dbConnectionVarName);
    }
}
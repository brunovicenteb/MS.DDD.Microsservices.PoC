using Toolkit.OutBox.Consumer;
using Toolkit.OutBox.Producer;
using Toolkit.TransactionalOutBox;
using Microsoft.AspNetCore.Builder;

namespace Toolkit.OutBox;

public static class OutBoxFactory
{
    public static ILogable BeginProducer<T>(this WebApplicationBuilder builder,
        bool recreateDb = false, string dbTypeVarName = "DATABASE_TYPE", string dbConnectionVarName = "DATABASE_CONNECTION") where T : OutBoxDbContext
    {
        return new ProducerOutBoxStarter<T>(builder, dbTypeVarName, recreateDb, dbConnectionVarName);
    }

    public static ILogable BeginConsumer<T>(this WebApplicationBuilder builder, string dbTypeVarName = "DATABASE_TYPE",
        string dbConnectionVarName = "DATABASE_CONNECTION") where T : OutBoxDbContext, new()
    {
        return new ConsumerOutBoxStarter<T>(builder, dbTypeVarName, dbConnectionVarName);
    }
}
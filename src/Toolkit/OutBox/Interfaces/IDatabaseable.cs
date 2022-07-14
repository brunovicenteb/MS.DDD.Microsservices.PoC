using Toolkit.TransactionalOutBox;

namespace Toolkit.OutBox.Interfaces;

public interface IDatabaseable
{
    public IBrokeable UseDatabase(DatabaseType databaseType, bool recreateDatabase = false, string dbConnectionVariableName = "DATABASE_CONNECTION");
}
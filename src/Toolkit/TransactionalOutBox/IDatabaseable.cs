using Microsoft.EntityFrameworkCore;

namespace Toolkit.TransactionalOutBox;

public interface IDatabaseable
{
    public IBrokeable UsePostgres(bool recreateDatabase);

    public IBrokeable UseSqlServer(bool recreateDatabase);
}
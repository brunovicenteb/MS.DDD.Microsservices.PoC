using Toolkit.TransactionalOutBox;

namespace Toolkit.OutBox;

public interface IDatabaseable
{
    public IBrokeable UseDatabase();
}
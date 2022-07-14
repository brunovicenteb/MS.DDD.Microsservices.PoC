using Microsoft.AspNetCore.Builder;
using Toolkit.OutBox.Interfaces;
using Toolkit.TransactionalOutBox;

namespace Toolkit.OutBox;

public static class TransactionalOutBoxFactory
{
    public static ILogable UseTransactionalOutBox<T>(this WebApplicationBuilder builder) where T : TransactionalOutBoxDbContext
    {
        return new TransactionalOutBox<T>(builder);
    }
}
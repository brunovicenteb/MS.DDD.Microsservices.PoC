using Toolkit.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Toolkit.TransactionalOutBox;

public static class TransactionalOutBoxFactory
{
    public static ILogable UseTransactionalOutBox<T>(this WebApplicationBuilder builder) where T : TransactionalOutBoxDbContext
    {
        return new TransactionalOutBox<T>(builder);
    }
}
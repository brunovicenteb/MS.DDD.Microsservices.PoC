using Microsoft.EntityFrameworkCore;
using Toolkit.RelationalDb;

namespace Benefit.Test.Toolkit.MessageBroker.Mock.Infra;
public class ObjectRepositoryMock : RelationalDbRepository<ObjectContextMock, ObjectMock>, IObjectRepositoryMock
{
    public ObjectRepositoryMock(ObjectContextMock context)
    : base(context)
    {
    }

    protected override DbSet<ObjectMock> Collection => Context.objects;
}
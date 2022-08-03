using Benefit.Test.Toolkit.MessageBroker.Mock.Infra;
using Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Contract.States;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Toolkit.Interfaces;
using Toolkit.Mapper;
using Toolkit.MessageBroker;

namespace Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Workers;
public class ObjectCreatedConsumerMock : BrokerConsumer<ObjectCreatedMock>
{
    public ObjectCreatedConsumerMock(IPublishEndpoint publisher, ILogger<ObjectCreatedConsumerMock> logger, IObjectRepositoryMock objectRepository)
    {
        _Publisher = publisher;
        _Logger = logger;
        _ObjectRepository = objectRepository;
        _Mapper = MapperFactory.Map<ObjectCreatedMock, ObjectNotifyFinishedMock>();
    }

    private readonly ILogger _Logger;
    private readonly IPublishEndpoint _Publisher;
    private readonly IObjectRepositoryMock _ObjectRepository;
    private readonly IGenericMapper _Mapper;

    protected override ILogger Logger => _Logger;

    protected override async Task ConsumeAsync(ObjectCreatedMock message)
    {
        var evt = _Mapper.Map<ObjectCreatedMock, ObjectNotifyFinishedMock>(message);
        await _Publisher.Publish(evt);
        await _ObjectRepository.SaveChangesAsync();
    }
}
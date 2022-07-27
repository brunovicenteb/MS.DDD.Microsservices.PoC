using Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Contract;
using Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Contract.States;
using MassTransit;

namespace Benefit.Test.Toolkit.MessageBroker.Mock.Sagas;
public class ObjectStateMachineMock : MassTransitStateMachine<ObjectStateMock>
{
    public ObjectStateMachineMock()
    {
        InstanceState(x => x.CurrentState);

        Initially(
            When(SubmitObjectEvent)
                    .Then(context =>
                    {
                        context.Saga.CorrelationId = context.Message.CorrelationId;
                        context.Saga.Name = context.Message.Name;
                    })
                    .TransitionTo(SubmittedState)
                    .Publish(context => new ObjectCreatedMock
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        Name = context.Message.Name,
                    }));

        During(SubmittedState,
                When(NotifyFinishedEvent)
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.CorrelationId;
                    context.Saga.Name = context.Message.Name;
                })
                .Finalize());
    }

    public State SubmittedState { get; private set; }
    //public State ObjectCreatedState { get; private set; }
    public Event<ObjectSubmittedMock> SubmitObjectEvent { get; } = null!;
    public Event<ObjectCreatedMock> CreatedObjectEvent { get; } = null!;
    public Event<ObjectNotifyFinishedMock> NotifyFinishedEvent { get; } = null!;
}
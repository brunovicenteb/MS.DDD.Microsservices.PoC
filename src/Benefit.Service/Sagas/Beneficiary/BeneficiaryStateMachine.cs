using MassTransit;
using Benefit.Service.Sagas.Beneficiary.Contract;

namespace Benefit.Service.Sagas.Beneficiary;

public class BeneficiaryStateMachine : MassTransitStateMachine<BeneficiaryState>
{
    public BeneficiaryStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Initially(
            When(SubmitBeneficiaryEvent)
                    .Then(context =>
                    {
                        context.Saga.CorrelationId = context.Message.CorrelationId;
                        context.Saga.Name = context.Message.Name;
                        context.Saga.CPF = context.Message.CPF;
                    })
                    .TransitionTo(SubmittedState)
                    .Publish(context => new BeneficiaryRegistered
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        Name = context.Message.Name,
                        CPF = context.Message.CPF
                    }));

        During(SubmittedState,
             When(IntegrateImdbEvent)
             .Then(context =>
             {
                 context.Saga.CorrelationId = context.Message.CorrelationId;
                 context.Saga.Name = context.Message.Name;
                 context.Saga.CPF = context.Message.CPF;
             })
             .TransitionTo(ImdbIntegratedState));

        During(ImdbIntegratedState,
             When(TheAudioDbIntegratedEvent)
             .Then(context =>
             {
                 context.Saga.CorrelationId = context.Message.CorrelationId;
                 context.Saga.Name = context.Message.Name;
                 context.Saga.CPF = context.Message.CPF;
             })
             .TransitionTo(TheAudioDbIntegratedState));

        During(TheAudioDbIntegratedState,
             When(NotifyFinishedEvent)
             .Then(context =>
             {
                 context.Saga.CorrelationId = context.Message.CorrelationId;
                 context.Saga.Name = context.Message.Name;
                 context.Saga.CPF = context.Message.CPF;
             })
             .Finalize());
    }

    public State SubmittedState { get; private set; }
    public State ImdbIntegratedState { get; private set; }
    public State TheAudioDbIntegratedState { get; private set; }
    public Event<BeneficiarySubmitted> SubmitBeneficiaryEvent { get; } = null!;
    public Event<BeneficiaryImdbIntegrated> IntegrateImdbEvent { get; } = null!;
    public Event<BeneficiaryTheAudioDbIntegrated> TheAudioDbIntegratedEvent { get; } = null!;
    public Event<BeneficiaryNotifyFinished> NotifyFinishedEvent { get; } = null!;
}
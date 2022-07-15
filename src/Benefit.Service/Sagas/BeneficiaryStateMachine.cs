using MassTransit;
using Benefit.Service.IoC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Benefit.Domain.AggregatesModel.Benefit;

public class BeneficiaryStateDefinition : SagaDefinition<BeneficiaryState>
{
    private readonly IServiceProvider _Provider;

    public BeneficiaryStateDefinition(IServiceProvider provider)
    {
        _Provider = provider;
    }
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<BeneficiaryState> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 1000, 1000, 1000, 1000, 1000));
        endpointConfigurator.UseEntityFrameworkOutbox<BenefitContext>(_Provider);
    }
}

public class BeneficiaryStateMap : SagaClassMap<BeneficiaryState>
{
    protected override void Configure(EntityTypeBuilder<BeneficiaryState> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState);
        entity.Property(x => x.Name);
        entity.Property(x => x.CPF);
    }
}

public class BeneficiaryState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public string Name { get; set; }
    public string CPF { get; set; }
}

public record IBeneficiaryState
{
    public Guid CorrelationId { get; set; }
    public string Name { get; set; }
    public string CPF { get; set; }
}

public record BeneficiarySubmitted : IBeneficiaryState
{
}

public record BeneficiaryRegistered : IBeneficiaryState
{
}

public record BeneficiaryImdbIntegred : IBeneficiaryState
{
}

public class BeneficiaryStateMachine : MassTransitStateMachine<BeneficiaryState>
{
    public BeneficiaryStateMachine()
    {
        Event(() => SubmitBeneficiary, x => x.CorrelateById(context => context.Message.CorrelationId));
        InstanceState(o => o.CurrentState);
        Initially(
            When(SubmitBeneficiary)
                    .Then(context =>
                    {
                        context.Saga.Name = context.Message.Name;
                        context.Saga.CPF = context.Message.CPF;
                    })
                    .TransitionTo(Submitted)
                    .Publish(context => new BeneficiaryRegistered
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        Name = context.Message.Name,
                        CPF = context.Message.CPF
                    }));

        //.TransitionTo(ImdbIntegrated));

        //During(Submitted,
        //    When(ImdbIntegratedBeneficiary)
        //    .TransitionTo(ImdbIntegrated));

        //During(Submitted,
        //    When(AnotherIntegratedBeneficiary)
        //    .TransitionTo(AnotherIntegrated)
        //    .Finalize());

        //    Initially(
        //When(RegistrationSubmitted)
        //    .Then(context =>
        //    {
        //        context.Saga.RegistrationDate = context.Message.RegistrationDate;
        //        context.Saga.EventId = context.Message.EventId;
        //        context.Saga.MemberId = context.Message.MemberId;
        //        context.Saga.Payment = context.Message.Payment;
        //    })
        //    .TransitionTo(Registered)
        //    .Publish(context => new SendRegistrationEmail
        //    {
        //        RegistrationId = context.Saga.CorrelationId,
        //        RegistrationDate = context.Saga.RegistrationDate,
        //        EventId = context.Saga.EventId,
        //        MemberId = context.Saga.MemberId
        //    })
        //    .If(context => context.Saga.Payment < 50m && context.GetRetryAttempt() == 0,
        //        fail => fail.Then(_ => throw new ApplicationException("Totally random, but you didn't pay enough for quality service")))
        //    .Publish(context => new AddEventAttendee
        //    {
        //        RegistrationId = context.Saga.CorrelationId,
        //        EventId = context.Saga.EventId,
        //        MemberId = context.Saga.MemberId
        //    })
    }

    public State Submitted { get; private set; }
    public State ImdbIntegrated { get; private set; }
    //public State AnotherIntegrated { get; private set; }
    public Event<BeneficiarySubmitted> SubmitBeneficiary { get; } = null!;
    public Event<BeneficiaryImdbIntegred> ImdbIntegratedBeneficiary { get; } = null!;
    //public Event<BeneficiaryAnotherIntegrated> AnotherIntegratedBeneficiary { get; } = null!;
}
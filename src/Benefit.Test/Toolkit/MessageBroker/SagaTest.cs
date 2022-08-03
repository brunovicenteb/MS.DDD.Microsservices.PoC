using MassTransit;
using Toolkit.OutBox;
using MassTransit.Testing;
using Benefit.Service.IoC;
using Benefit.Service.Workers;
using Toolkit.TransactionalOutBox;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Benefit.Service.Sagas.Beneficiary.Contract;
using Benefit.Domain.Interfaces;
using Benefit.Domain.Benefit;
using System.Threading.Tasks;
using Benefit.Service.Sagas.Beneficiary;

namespace Benefit.Test.Toolkit.MessageBroker;

public class SagaTest
{
    [Fact]
    public async void TestePublishAndConsumeTestMessage()
    {
        ITestHarness harness = null;
        ServiceProvider provider = null;
        try
        {
            //arrange
            provider = CreateProvider();
            var sagaId = NewId.NextGuid();
            harness = provider.GetRequiredService<ITestHarness>();
            var beneficiary = await CreateBeneficiary(provider, "Katie Bouma", "59006624039");
            await harness.Start();

            //act
            await harness.Bus.Publish(new BeneficiaryRegistered
            {
                CorrelationId = sagaId,
                Name = beneficiary.Name,
                CPF = beneficiary.CPF
            });

            //assert
            bool registeredMsg = await harness.Consumed.Any<BeneficiaryRegistered>();
            Assert.True(registeredMsg);

            bool imdbIntegratedMsg = await harness.Consumed.Any<BeneficiaryImdbIntegrated>();
            Assert.True(imdbIntegratedMsg);

            bool theAudioDbIntegratedMsg = await harness.Consumed.Any<BeneficiaryTheAudioDbIntegrated>();
            Assert.True(theAudioDbIntegratedMsg);

            bool finishedMsg = await harness.Consumed.Any<BeneficiaryNotifyFinished>();
            Assert.True(finishedMsg);

            //var sagaHarness = harness.GetSagaStateMachineHarness<BeneficiaryStateMachine, BeneficiaryState>();
            //var instance = sagaHarness.Created.ContainsInState(sagaId, sagaHarness.StateMachine, sagaHarness.StateMachine.Final);
            //Assert.Equal(beneficiary.Name, instance.Name);
            //Assert.Equal(beneficiary.CPF, instance.CPF);
            //Assert.Equal("Finished", instance.CurrentState);
        }
        finally
        {
            await harness?.Stop();
            await provider.DisposeAsync();
            Environment.SetEnvironmentVariable("InMemory", string.Empty);
        }
    }

    private async Task<Beneficiary> CreateBeneficiary(ServiceProvider serviceProvider, string name, string cpf)
    {
        var repository = serviceProvider.GetRequiredService<IBenefitRepository>();
        return await repository.AddAsync(new Beneficiary() { Name = name, CPF = cpf }, true);
    }

    private ServiceProvider CreateProvider()
    {
        Environment.SetEnvironmentVariable("InMemory", "InMemory");
        var dbType = DatabaseType.InMemory.ToString();
        var builder = WebApplication.CreateBuilder();
        builder.BeginProducer<BenefitContext>(dbTypeVarName: dbType)
            .UseSerilog()
            .UseTelemetry()
            .UseDatabase()
            .UseHarness();
        builder.BeginConsumer<BenefitContext>(dbTypeVarName: dbType)
            .UseSerilog()
            .UseTelemetry()
            .UseDatabase()
            .UseHarness();
        return builder.Services.BuildServiceProvider();
    }
}
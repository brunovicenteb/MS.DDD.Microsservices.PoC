using MassTransit;
using Benefit.Service.IoC;
using Benefit.Domain.Benefit;
using System.Threading.Tasks;
using Benefit.Domain.Interfaces;
using Benefit.Service.Sagas.Beneficiary;
using Microsoft.Extensions.DependencyInjection;
using Benefit.Service.Sagas.Beneficiary.Contract;
using Benefit.Test.Toolkit.MessageBroker;

namespace Benefit.Test.Benefit;

public class BeneficiarySagaTest : StateMachineTestFixture<BenefitContext, BeneficiaryStateMachine, BeneficiaryState>
{
    public BeneficiarySagaTest(StateMachineTestFixtureFixture<BenefitContext> fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async void TestePublishAndConsumeTestMessage()
    {
        //arrange
        var sagaId = NewId.NextGuid();
        var beneficiary = await CreateBeneficiary("Katie Bouma", "59006624039");

        //act
        await TestHarness.Bus.Publish(new BeneficiaryRegistered
        {
            CorrelationId = sagaId,
            Name = beneficiary.Name,
            CPF = beneficiary.CPF
        });

        //assert    
        bool registeredMsg = await TestHarness.Consumed.Any<BeneficiaryRegistered>();
        Assert.True(registeredMsg, "BeneficiaryRegistered Message not consumed by Beneficiary Saga.");

        bool imdbIntegratedMsg = await TestHarness.Consumed.Any<BeneficiaryImdbIntegrated>();
        Assert.True(imdbIntegratedMsg, "BeneficiaryImdbIntegrated Message not consumed by Beneficiary Saga.");

        bool theAudioDbIntegratedMsg = await TestHarness.Consumed.Any<BeneficiaryTheAudioDbIntegrated>();
        Assert.True(theAudioDbIntegratedMsg, "BeneficiaryTheAudioDbIntegrated Message not consumed by Beneficiary Saga.");

        bool finishedMsg = await TestHarness.Consumed.Any<BeneficiaryNotifyFinished>();
        Assert.True(finishedMsg, "BeneficiaryNotifyFinished Message not consumed by Beneficiary Saga.");

        //var sagaHarness = TestHarness.GetSagaStateMachineHarness<BeneficiaryStateMachine, BeneficiaryState>();
        //var instance = sagaHarness.Exists(sagaId);
        //var existsId = await sagaHarness.Exists(sagaId, o => o.SubmittedState);
        //Assert.True(existsId.HasValue, "Beneficiary Saga not pass thru SubmittedState.");

        //var existsId = await SagaHarness.Exists(sagaId, o => o.Final);
        //Assert.True(existsId.HasValue, "Beneficiary Saga not completed successfully.");
    }

    private async Task<Beneficiary> CreateBeneficiary(string name, string cpf)
    {
        var repository = Provider.GetRequiredService<IBenefitRepository>();
        return await repository.AddAsync(new Beneficiary() { Name = name, CPF = cpf }, true);
    }
}
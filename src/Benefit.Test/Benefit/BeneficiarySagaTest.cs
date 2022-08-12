using MassTransit;
using Benefit.Service.IoC;
using Benefit.Domain.Benefit;
using System.Threading.Tasks;
using Benefit.Domain.Interfaces;
using Benefit.Service.Sagas.Beneficiary;
using Microsoft.Extensions.DependencyInjection;
using Benefit.Service.Sagas.Beneficiary.Contract;

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
        await TestHarness.Bus.Publish(new BeneficiarySubmitted
        {
            CorrelationId = sagaId,
            Name = beneficiary.Name,
            CPF = beneficiary.CPF
        });

        //var timeOut = TimeSpan.FromSeconds(30);

        //assert    
        //var sagaHarness = TestHarness.GetSagaStateMachineHarness<BeneficiaryStateMachine, BeneficiaryState>();

        //var existsId = await sagaHarness.Exists(sagaId, o => o.SubmittedState, timeout: timeOut);
        //Assert.True(existsId.HasValue, $"SubmittedState not exist on SagaID {sagaId}");

        bool registeredMsg = await TestHarness.Consumed.Any<BeneficiaryRegistered>();
        Assert.True(registeredMsg, $"BeneficiaryRegistered Message not consumed by Beneficiary SagaID {sagaId}");

        bool imdbIntegratedMsg = await TestHarness.Consumed.Any<BeneficiaryImdbIntegrated>();
        Assert.True(imdbIntegratedMsg, $"BeneficiaryImdbIntegrated Message not consumed by Beneficiary SagaID {sagaId}");

        //existsId = await sagaHarness.Exists(sagaId, o => o.ImdbIntegratedState, timeout: timeOut);
        //Assert.True(existsId.HasValue, $"ImdbIntegratedState not exist on SagaID {sagaId}");

        bool theAudioDbIntegratedMsg = await TestHarness.Consumed.Any<BeneficiaryTheAudioDbIntegrated>();
        Assert.True(theAudioDbIntegratedMsg, $"BeneficiaryTheAudioDbIntegrated Message not consumed by Beneficiary SagaID {sagaId}");

        //existsId = await sagaHarness.Exists(sagaId, o => o.TheAudioDbIntegratedState, timeout: timeOut);
        //Assert.True(existsId.HasValue, $"TheAudioDbIntegratedState not exist on SagaID {sagaId}");

        bool finishedMsg = await TestHarness.Consumed.Any<BeneficiaryNotifyFinished>();
        Assert.True(finishedMsg, $"BeneficiaryNotifyFinished Message not consumed by Beneficiary SagaID {sagaId}");

        //existsId = await sagaHarness.Exists(sagaId, o => o.Final, timeout: timeOut);
        //Assert.True(existsId.HasValue, $"Final State not exist on  SagaID {sagaId}");
    }

    private async Task<Beneficiary> CreateBeneficiary(string name, string cpf)
    {
        var repository = Provider.GetRequiredService<IBenefitRepository>();
        return await repository.AddAsync(new Beneficiary() { Name = name, CPF = cpf }, true);
    }
}
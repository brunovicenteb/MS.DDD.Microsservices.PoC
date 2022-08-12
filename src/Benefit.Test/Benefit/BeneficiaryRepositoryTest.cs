using System.Linq;
using Benefit.Service.IoC;
using Benefit.Service.Infra;
using Benefit.Domain.Benefit;
using Benefit.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Xml.Linq;

namespace Benefit.Test.Benefit;

public class BeneficiaryRepositoryTest : StarterIoC<BenefitContext>
{

    [Fact]
    public async void TestCountAsync()
    {
        //arrange
        var repository = Provider.GetRequiredService<IBenefitRepository>();

        //act
        await repository.AddAsync(new Beneficiary() { Name = "Camila Queiroz", CPF = "44074830086" });
        await repository.AddAsync(new Beneficiary() { Name = "Marina Ruy Barbosa", CPF = "60220546053" });
        var count = await repository.CountAsync();

        //assert
        Assert.True(count > 0);
    }

    [Fact]
    public async void TestDeleteAsync()
    {
        //arrange
        var repository = Provider.GetRequiredService<IBenefitRepository>();
        var beneficiary = await repository.AddAsync(new Beneficiary() { Name = "Paolla Oliveira", CPF = "53872673068" });
        var count = await repository.CountAsync();
        var r = (BenefitRepository)repository;
        r.Context.ChangeTracker.Clear();

        //act
        await repository.DeleteAsync(beneficiary.ID);
        var countAfterDelete = await repository.CountAsync();
        var loadedBeneficiary = await repository.GetObjectByIDAsync(beneficiary.ID);

        //assert
        Assert.Null(loadedBeneficiary);
        Assert.Equal(count - 1, countAfterDelete);
    }

    [Fact]
    public async void TestDeleteAsyncAppySaveFalse()
    {
        //arrange
        var repository = Provider.GetRequiredService<IBenefitRepository>();
        var beneficiary = await repository.AddAsync(new Beneficiary() { Name = "Izabel Goulart", CPF = "93650992000" });
        var count = await repository.CountAsync();
        var r = (BenefitRepository)repository;
        r.Context.ChangeTracker.Clear();

        //act
        await repository.DeleteAsync(beneficiary.ID, false);
        var countAfterDelete = await repository.CountAsync();
        var loadedBeneficiary = await repository.GetObjectByIDAsync(beneficiary.ID);

        //assert
        Assert.NotNull(loadedBeneficiary);
        Assert.Equal(count, countAfterDelete);
        await repository.SaveChangesAsync();
        countAfterDelete = await repository.CountAsync();
        loadedBeneficiary = await repository.GetObjectByIDAsync(beneficiary.ID);
        Assert.Null(loadedBeneficiary);
        Assert.Equal(count - 1, countAfterDelete);
    }

    [Fact]
    public async void TestGetObjectByID()
    {
        //arrange
        var repository = Provider.GetRequiredService<IBenefitRepository>();
        var beneficiary = await repository.AddAsync(new Beneficiary() { Name = "Taís Araújo", CPF = "11790027098" });

        //act
        var loadedBeneficiary = await repository.GetObjectByIDAsync(beneficiary.ID);

        //assert
        Assert.NotNull(loadedBeneficiary);
        Assert.NotSame(beneficiary, loadedBeneficiary);
        Assert.Equal("Taís Araújo", beneficiary.Name);
        Assert.Equal("11790027098", beneficiary.CPF);
    }

    [Fact]
    public async void TestGetObjectByIDAsyncNotExists()
    {
        //arrange
        var repository = Provider.GetRequiredService<IBenefitRepository>();

        //act
        var beneficiary = await repository.GetObjectByIDAsync(int.MaxValue);

        //assert
        Assert.Null(beneficiary);
    }

    [Theory]
    [InlineData("Cleo Pires", "53607274037")]
    [InlineData("Juliana Paes", "94193574067")]
    public async void TestAddAsync(string name, string cpf)
    {
        //arrange
        var repository = Provider.GetRequiredService<IBenefitRepository>();

        //act
        var beneficiary = await repository.AddAsync(new Beneficiary() { Name = name, CPF = cpf });

        //assert
        Assert.NotNull(beneficiary);
        Assert.Equal(name, beneficiary.Name);
        Assert.Equal(cpf, beneficiary.CPF);
    }

    [Theory]
    [InlineData("Isis Valverde", "96294188008")]
    [InlineData("Grazi Massafera", "97676453062")]
    public async void TestGetAsync(string name, string cpf)
    {
        //arrange
        var repository = Provider.GetRequiredService<IBenefitRepository>();
        var beneficiary = await repository.AddAsync(new Beneficiary() { Name = name, CPF = cpf });

        //act
        var loadedBeneficiary = await repository.GetObjectByIDAsync(beneficiary.ID);

        //assert
        Assert.NotSame(beneficiary, loadedBeneficiary);
        Assert.Equal(name, loadedBeneficiary.Name);
        Assert.Equal(cpf, loadedBeneficiary.CPF);
    }

    [Theory]
    [InlineData("Bárbara Evans", "60429564007")]
    [InlineData("Isabeli Fontana", "19033219050")]
    public async void TestAddAsyncAppySaveFalse(string name, string cpf)
    {
        //arrange
        var repository = Provider.GetRequiredService<IBenefitRepository>();

        //act
        var beneficiary = await repository.AddAsync(new Beneficiary() { Name = name, CPF = cpf }, false);
        var r = (BenefitRepository)repository;
        r.Context.SaveChanges();
        var allBeneficiaries = await repository.GetAsync();
        var loadedBeneficiary = allBeneficiaries.FirstOrDefault(o => o.Name == name);

        //assert
        Assert.Null(beneficiary);
        Assert.NotNull(loadedBeneficiary);
        Assert.Equal(name, loadedBeneficiary.Name);
        Assert.Equal(cpf, loadedBeneficiary.CPF);
    }
}
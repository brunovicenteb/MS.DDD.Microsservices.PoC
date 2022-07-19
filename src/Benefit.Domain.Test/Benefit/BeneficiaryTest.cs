using Benefit.Domain.Benefit;

namespace Benefit.Domain.Test.Benefit;

public class BeneficiaryTest
{
    [Theory]
    [InlineData("1", "John Snow", "31018589090", OperatorType.Hapvida, "1991-11-13")]
    [InlineData("2", "Sansa Stark", "54216025080", OperatorType.Amil, "1967-05-06")]
    [InlineData("3", "Tyrion Lannister", "88421597000", OperatorType.Unimed, "1988-01-03")]
    [InlineData("4", "Robert Baratheon", "", OperatorType.Hapvida, "1976-07-04")]
    public void TestCreator(string id, string name, string cpf, OperatorType operatorType, DateTime birthDate)
    {
        DateTime now = DateTime.Now;
        var beneficiary = new Beneficiary(id, operatorType, name, cpf, birthDate, now, null, null);
        Assert.Equal(id, beneficiary.ID);
        Assert.Equal(name, beneficiary.Name);
        Assert.Equal(cpf, beneficiary.CPF);
        Assert.Equal(operatorType, beneficiary.Operator);
        Assert.Equal(birthDate, beneficiary.BirthDate);
        Assert.Equal(now, beneficiary.CreateAt);
        Assert.False(beneficiary.DeletedAt.HasValue);
    }
}
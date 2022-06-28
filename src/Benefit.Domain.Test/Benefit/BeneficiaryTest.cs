using Benefit.Domain.Benefit;
using Benefit.Domain.Operator;

namespace Benefit.Domain.Test.Benefit;

public class BeneficiaryTest
{
    [Fact]
    public void TestCreator()
    {
        DateTime now = DateTime.Now;
        var beneficiary = new Beneficiary(1, null, OperatorType.Hapvida,
            "John Snow", "31018589090", new DateTime(1991, 11, 3), now, null, null);
        Assert.Equal((uint)1, beneficiary.ID);
        Assert.False(beneficiary.ParentID.HasValue);
        Assert.Equal(OperatorType.Hapvida, beneficiary.Operator);
        Assert.Equal("John Snow", beneficiary.Name);
        Assert.Equal("31018589090", beneficiary.CPF);
        Assert.Equal(new DateTime(1991, 11, 3), beneficiary.BirthDate);
        Assert.Equal(now, beneficiary.CreateAt);
        Assert.False(beneficiary.DeletedAt.HasValue);
    }
}
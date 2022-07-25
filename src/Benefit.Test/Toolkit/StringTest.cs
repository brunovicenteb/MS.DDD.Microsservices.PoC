using System.Linq;
using Toolkit;

namespace Benefit.Test.Toolkit;
public class StringTest
{
    [Theory]
    [InlineData("Has value!", false)]
    [InlineData("", true)]
    [InlineData(" ", false)]
    [InlineData("Also has value!", false)]
    [InlineData(null, true)]
    public void IsEmptyValueFunctionTest(string valueForTest, bool expectedValue)
    {
        //arrange
        //act
        var result = valueForTest.IsEmpty();

        //assert
        Assert.IsType<bool>(result);
        Assert.Equal(expectedValue, result);

        if (result)
            Assert.True(String.IsNullOrEmpty(valueForTest));
        else
            Assert.NotEmpty(valueForTest);
    }

    [Theory]
    [InlineData("Has value!", true)]
    [InlineData("", false)]
    [InlineData(" ", true)]
    [InlineData("Also has value!", true)]
    [InlineData(null, false)]
    public void IsFilledValueFunctionTest(string valueForTest, bool expectedValue)
    {
        //arrange
        //act
        var result = valueForTest.IsFilled();

        //assert
        Assert.IsType<bool>(result);
        Assert.Equal(expectedValue, result);

        if (result)
            Assert.NotEmpty(valueForTest);
        else
            Assert.True(String.IsNullOrEmpty(valueForTest));
    }

    [Theory]
    [InlineData("is lower!", "is lower!")]
    [InlineData("IS UPPER", "is upper")]
    [InlineData("iT's bOth", "it's both")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void SafeToLowerFunctionTest(string valueForTest, string expectedValue)
    {
        //arrange
        //act
        var result = valueForTest.SafeToLower();
        var resultOnlyLetters = new String(result.Where(c => Char.IsLetter(c)).ToArray());

        //assert
        Assert.IsType<string>(result);
        Assert.True(resultOnlyLetters.All(char.IsLower));
        Assert.Equal(expectedValue, result);
    }

    [Theory]
    [InlineData("481.431.670-46")]
    [InlineData("890.564.540-21")]
    [InlineData("496.851.180-94")]
    [InlineData("692.144.560-70")]
    public void ValidCpfTest(string valueForTest)
    {
        //arrange
        //act
        var validDocument = valueForTest.IsValidCPF();

        //assert
        Assert.IsType<bool>(validDocument);
        Assert.True(validDocument);
    }

    [Theory]
    [InlineData("481.431.670")]
    [InlineData("190.564.540-21")]
    [InlineData("557.851.180-94")]
    [InlineData("")]
    [InlineData(null)]
    public void InvalidCpfTest(string valueForTest)
    {
        //arrange
        //act
        var validDocument = valueForTest.IsValidCPF();

        //assert
        Assert.IsType<bool>(validDocument);
        Assert.False(validDocument);
    }

}
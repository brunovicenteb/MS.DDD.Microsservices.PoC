using System.Linq;
using Toolkit;

namespace Benefit.Test.Toolkit
{
    public class StringTest
    {
        [Theory]
        [InlineData("Has value!")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Also has value!")]
        public void TestEmptyValueFunction(string valueForTest)
        {
            //arrange

            //act
            var result = Strings.IsEmpty(valueForTest);

            //assert
            Assert.IsType<bool>(result);

            if (result)
                Assert.Empty(valueForTest);
            else
                Assert.NotEmpty(valueForTest);
        }

        [Theory]
        [InlineData("Has value!")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Also has value!")]
        public void TestFilledValueFunction(string valueForTest)
        {
            //arrange

            //act
            var result = Strings.IsFilled(valueForTest);

            //assert
            Assert.IsType<bool>(result);

            if (result)
                Assert.NotEmpty(valueForTest);
            else
                Assert.Empty(valueForTest);
        }

        [Theory]
        [InlineData("is lower!")]
        [InlineData("IS UPPER")]
        [InlineData("iT's bOth")]
        [InlineData(null)]
        public void TestSafeToLowerFunction(string valueForTest)
        {
            //arrange

            //act
            var result = Strings.SafeToLower(valueForTest);
            var resultOnlyLetters = new String(result.Where(c => Char.IsLetter(c)).ToArray());

            //assert
            Assert.IsType<string>(result);
            Assert.True(resultOnlyLetters.All(char.IsLower));
        }

        [Theory]
        [InlineData("481.431.670-46")]
        [InlineData("890.564.540-21")]
        [InlineData("496.851.180-94")]
        [InlineData("692.144.560-70")]
        public void TestValidCpf(string valueForTest)
        {
            //arrange

            //act
            var validDocument = Strings.IsValidCPF(valueForTest);

            //assert
            Assert.IsType<bool>(validDocument);
            Assert.True(validDocument);
        }

        [Theory]
        [InlineData("481.431.670")]
        [InlineData("190.564.540-21")]
        [InlineData("557.851.180-94")]
        [InlineData("")]
        public void TestInvalidCpf(string valueForTest)
        {
            //arrange

            //act
            var validDocument = Strings.IsValidCPF(valueForTest);

            //assert
            Assert.IsType<bool>(validDocument);
            Assert.False(validDocument);
        }

    }
}

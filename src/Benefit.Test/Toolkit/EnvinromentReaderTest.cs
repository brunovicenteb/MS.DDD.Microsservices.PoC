using Toolkit;

namespace Benefit.Test.Toolkit;

public class EnvinromentReaderTest
{
    [Theory]
    [InlineData("")]
    [InlineData("Ping")]
    public void TestReadEmptyStringVariableWitDefaultValue(string defaultValue)
    {
        //arrange
        var varName = nameof(TestReadStringVariableSucessWithDefaultValue);
        Environment.SetEnvironmentVariable(varName, defaultValue);
        //act
        string value = EnvinromentReader.Read(varName, defaultValue);
        //assert
        Assert.Equal(value, defaultValue);
    }

    [Theory]
    [InlineData(25)]
    [InlineData(45)]
    public void TestReadEmptyIntVariableWitDefaultValue(int defaultValue)
    {
        //arrange
        var varName = nameof(TestReadEmptyIntVariableWitDefaultValue);
        Environment.SetEnvironmentVariable(varName, string.Empty);
        //act
        int value = EnvinromentReader.Read(varName, defaultValue);
        //assert
        Assert.Equal(value, defaultValue);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]

    public void TestReadWithoutVarName(string varName)
    {
        //arrange
        //act
        var exception = Assert.Throws<ArgumentNullException>(() => EnvinromentReader.Read<string>(varName));
        //assert
        Assert.Equal("Value cannot be null. (Parameter 'varName')", exception.Message);
    }

    [Theory]
    [InlineData("Ping")]
    [InlineData("Pong")]
    public void TestReadStringVariableSucess(string varValue)
    {
        var varName = nameof(TestReadStringVariableSucessWithDefaultValue);
        try
        {
            //arrange
            Environment.SetEnvironmentVariable(varName, varValue);
            //act
            string value = EnvinromentReader.Read<string>(varName);
            //assert
            Assert.Equal(varValue, value);
        }
        finally
        {
            Environment.SetEnvironmentVariable(varName, null);
        }
    }

    [Theory]
    [InlineData("Ping", "Pont")]
    [InlineData("Tico", "Teco")]
    public void TestReadStringVariableSucessWithDefaultValue(string varValue, string defaultValue)
    {
        var varName = nameof(TestReadStringVariableSucessWithDefaultValue);
        try
        {
            //arrange
            Environment.SetEnvironmentVariable(varName, varValue);
            //act
            var value = EnvinromentReader.Read(varName, defaultValue);
            //assert
            Assert.Equal(varValue, value);
        }
        finally
        {
            Environment.SetEnvironmentVariable(varName, null);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void TestIntVariableSucess(int varValue)
    {
        var varName = nameof(TestIntVariableSucess);
        try
        {
            //arrange
            Environment.SetEnvironmentVariable(varName, varValue.ToString());
            //act
            var value = EnvinromentReader.Read<int>(varName);
            //assert
            Assert.Equal(varValue, value);
        }
        finally
        {
            Environment.SetEnvironmentVariable(varName, null);
        }
    }

    [Fact]
    public void TestRaiseExceptionWithUnassignedVariable()
    {
        //arrange
        var varName = nameof(TestRaiseExceptionWithUnassignedVariable);
        string message = $"Variable {varName} not assigned on environment";

        //act
        var exception = Assert.Throws<NullReferenceException>(() => EnvinromentReader.Read<string>(varName, varEmptyError: message));

        //assert
        Assert.Equal(message, exception.Message);
    }

    [Theory]
    [InlineData("Ping")]
    [InlineData("P0nG")]
    public void TestRaiseExceptionWithFormatException(string varValue)
    {
        var varName = nameof(TestRaiseExceptionWithFormatException);
        try
        {
            //arrange
            Environment.SetEnvironmentVariable(varName, varValue);
            //act
            var exception = Assert.Throws<FormatException>(() => EnvinromentReader.Read<int>(varName));
            //assert
            Assert.Equal("Input string was not in a correct format.", exception.Message);
        }
        finally
        {
            Environment.SetEnvironmentVariable(varName, null);
        }
    }
}
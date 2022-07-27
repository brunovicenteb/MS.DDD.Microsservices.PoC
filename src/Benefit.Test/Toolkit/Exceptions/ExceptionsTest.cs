using Benefit.Test.Toolkit.Exceptions.Mock;
using System.Collections.Generic;
using Toolkit.Exceptions;
using Xunit;

namespace Benefit.Test.Toolkit.Exceptions;
public class ExceptionsTest
{
    [Theory]
    [ClassData(typeof(ExceptionsMock))]
    public void GetCustomExceptionTest(BaseException exception)
    {
        //arrange
        //act
        var excetionType = exception.GetType();
        var excetionName = exception.Message;

        //assert
        Assert.Equal(typeof(BaseException), excetionType.BaseType);
        Assert.Equal(typeof(Exception), excetionType.BaseType.BaseType);
        Assert.IsType(excetionType, exception);
        Assert.Contains(excetionType.Name, excetionName);
    }
}
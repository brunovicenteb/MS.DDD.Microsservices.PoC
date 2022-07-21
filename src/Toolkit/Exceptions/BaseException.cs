namespace Toolkit.Exceptions;

public abstract class BaseException : Exception
{
    protected BaseException()
    {
    }

    protected BaseException(string message, Exception innerException)
    : base(message, innerException)
    {
    }

    public BaseException(string pMessage)
        : base(pMessage)
    {
    }
}
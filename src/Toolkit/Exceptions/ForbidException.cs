namespace Toolkit.Exceptions;

public sealed class ForbidException : BaseException
{
    public ForbidException(string pMessage)
        : base(pMessage)
    {
    }
}
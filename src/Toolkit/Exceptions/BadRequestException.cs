namespace Toolkit.Exceptions;

public sealed class BadRequestException : BaseException
{
    public BadRequestException(string pMessage)
        : base(pMessage)
    {
    }
}
namespace Toolkit.Exceptions;

public sealed class NotFoundException : BaseException
{
    public NotFoundException(string pMessage)
        : base(pMessage)
    {
    }
}
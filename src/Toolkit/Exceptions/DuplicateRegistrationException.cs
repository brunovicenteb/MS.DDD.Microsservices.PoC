using System.Runtime.Serialization;

namespace Toolkit.Exceptions;

[Serializable]
public sealed class DuplicateRegistrationException : BaseException
{
    public DuplicateRegistrationException()
    {
    }

    public DuplicateRegistrationException(string message)
        : base(message)
    {
    }

    public DuplicateRegistrationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
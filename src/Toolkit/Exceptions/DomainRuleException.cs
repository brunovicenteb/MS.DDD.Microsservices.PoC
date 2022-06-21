namespace Toolkit.Exceptions;

public sealed class DomainRuleException : BaseException
{
    public DomainRuleException(string mensagem)
        : base(mensagem)
    {
    }
}
using Toolkit;
using System.Text;
using Toolkit.Exceptions;
using Benefit.Domain.Operator;
using Benefit.Domain.Benefit;

public abstract class BaseOperator
{
    private static uint Count;
    protected abstract OperatorType Type { get; }
    protected virtual bool IsRequiredCPF(Beneficiary beneficiary) => true;

    public Beneficiary CreateBeneficiary(uint? parentID, string name, string cpf, DateTime? birthDate)
    {
        var erros = new StringBuilder();
        uint nextID = Interlocked.Increment(ref Count);
        var beneficiary = new Beneficiary(nextID, parentID, Type, name, cpf, birthDate, DateTime.UtcNow, null, null);
        beneficiary.Validate(erros);
        ValidateBeneficiary(erros, beneficiary);
        var foundedErros = erros.ToString();
        if (foundedErros.IsFilled())
            throw new DomainRuleException(foundedErros);
        return beneficiary;
    }

    private void ValidateBeneficiary(StringBuilder erros, Beneficiary beneficiary)
    {
        if (beneficiary.CPF.IsEmpty() && IsRequiredCPF(beneficiary))
            erros.AppendLine("The field \"CPF\" cannot be empty.");
    }
}
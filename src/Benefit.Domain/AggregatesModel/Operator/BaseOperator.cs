using Toolkit;
using System.Text;
using Toolkit.Exceptions;
using Benefit.Domain.Benefit;

namespace Benefit.Domain.Operator;
public abstract class BaseOperator
{
    protected abstract OperatorType Type { get; }
    protected virtual bool IsRequiredCPF(Beneficiary beneficiary) => true;

    public Beneficiary CreateBeneficiary(string name, string cpf, DateTime? birthDate)
    {
        var erros = new StringBuilder();
        var beneficiary = new Beneficiary(0, Type, name, cpf, birthDate, DateTime.UtcNow, null, null);
        beneficiary.Validate(erros);
        ValidateBeneficiary(erros, beneficiary);
        var foundedErros = erros.ToString();
        if (foundedErros.IsFilled())
            throw new DomainRuleException(foundedErros);
        return beneficiary;
    }

    private void ValidateBeneficiary(StringBuilder erros, Beneficiary beneficiary)
    {
        //if (beneficiary.CPF.IsEmpty() && IsRequiredCPF(beneficiary))
        //    erros.AppendLine("The field \"CPF\" cannot be empty.");
    }
}
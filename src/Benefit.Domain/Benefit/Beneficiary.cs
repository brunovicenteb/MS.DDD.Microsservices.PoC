using Toolkit;
using System.Text;
using Toolkit.Data;
using FluentValidation;
using Benefit.Domain.Benefit.Rules;

namespace Benefit.Domain.Benefit;
public class Beneficiary : TimeCycleEntity
{
    public Beneficiary()
        : base()
    {
    }

    public Beneficiary(string id, OperatorType operatorType, string name, string cpf, DateTime? birthDate,
        DateTime createAt, DateTime? updateAt, DateTime? deletedAt)
        : base(id, createAt, updateAt, deletedAt)
    {
        Operator = operatorType;
        Name = name;
        CPF = cpf;
        BirthDate = birthDate;
    }

    public OperatorType Operator { get; set; }
    public string Name { get; set; }
    public string CPF { get; set; }
    public DateTime? BirthDate { get; set; }

    public bool IsUnderAge
    {
        get
        {
            if (BirthDate == null)
                return false;
            return BirthDate.Value.AddYears(18) > DateTime.Now;
        }
    }

    public override IValidator[] GetValidators()
    {
        return new IValidator[] { new BeneficiaryValidator() };
    }

    public void Validate(StringBuilder errors)
    {
        if (Name.IsEmpty())
            errors.AppendLine("The field \"Name\" cannot be empty.");
        if (CPF.IsFilled() && !CPF.IsValidCPF())
            errors.AppendLine("The field \"CPF\" is not valid.");
        if (BirthDate.HasValue && BirthDate.Value > DateTime.Now)
            errors.AppendLine("The field \"BirthDate\" cannot be in the future.");
    }
}
using Toolkit;
using System.Text;
using Benefit.Domain.Operator;

namespace Benefit.Domain.BeneficiaryAggregate;
public class Beneficiary
{
    public Beneficiary()
    { 
    }

    public Beneficiary(uint id, uint? parentID, OperatorType operatorType, string name, string cpf, DateTime? birthDate)
    {
        ID = id;
        ParentID = parentID;
        Operator = operatorType;
        Name = name;
        CPF = cpf;
        BirthDate = birthDate;
    }

    public uint ID { get; set; }
    public uint? ParentID { get; set; }
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

    public bool IsTitular
    {
        get
        {
            return !ParentID.HasValue;
        }
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
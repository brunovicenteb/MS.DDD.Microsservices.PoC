using Benefit.Domain.Operator;

namespace Benefit.API.ValueObject;
public class BeneficiaryVO
{
    public uint? ParentID { get; set; }
    public OperatorType Operator { get; set; }
    public string Name { get; set; }
    public string CPF { get; set; }
    public DateTime? BirthDate { get; set; }
}
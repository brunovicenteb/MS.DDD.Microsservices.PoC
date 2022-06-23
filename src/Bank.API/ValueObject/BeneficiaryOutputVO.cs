using Benefit.Domain.Operator;
using Benefit.Domain.BeneficiaryAggregate;

namespace Benefit.API.ValueObject;
public class BeneficiaryOutputVO
{
    public BeneficiaryOutputVO()
    {
    }

    public BeneficiaryOutputVO(Beneficiary Beneficiary)
    {
        ID = Beneficiary.ID;
        ParentID = Beneficiary.ParentID;
        Operator = Beneficiary.Operator;
        Name = Beneficiary.Name;
        CPF = Beneficiary.CPF;
        BirthDate = Beneficiary.BirthDate;
        CreateAt = Beneficiary.CreateAt;
        DeletedAt = Beneficiary.DeletedAt;
        UpdateAt = Beneficiary.UpdateAt;
    }

    public uint ID { get; set; }
    public uint? ParentID { get; set; }
    public OperatorType Operator { get; set; }
    public string Name { get; set; }
    public string CPF { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? UpdateAt { get; set; }
}
using Benefit.Domain.Operator;

namespace Benefit.API.DTO.Beneficiary
{
    public class BeneficiaryInput
    {
        public uint? ParentID { get; set; }
        public OperatorType Operator { get; set; }
        public string Name { get; set; }
        public string CPF { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}

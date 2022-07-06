using Benefit.Domain.Benefit;

namespace Benefit.API.DTO;
public class BeneficiaryCreateRequest
{
    public OperatorType Operator { get; set; }
    public string Name { get; set; }
    public string CPF { get; set; }
    public DateTime? BirthDate { get; set; }
}
using Benefit.Domain.Benefit;

namespace Benefit.API.DTO;
public class BeneficiaryResponse
{
    public int ID { get; set; }
    public OperatorType Operator { get; set; }
    public string Name { get; set; }
    public string CPF { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public BeneficiaryImdbWorkResponse[] ImdbWorks { get; set; }
    public BeneficiaryTheAudioDbWorkResponse[] TheAudioDbWorks { get; set; }
}
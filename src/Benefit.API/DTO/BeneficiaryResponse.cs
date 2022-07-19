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

public class BeneficiaryImdbWorkResponse
{
    public string Title { get; set; }
    public string Image { get; set; }
    public string Description { get; set; }
}

public class BeneficiaryTheAudioDbWorkResponse
{
    public string IdArtist { get; set; }
    public string Artist { get; set; }
    public string ArtistAlternate { get; set; }
    public string Label { get; set; }
    public string FormedYear { get; set; }
    public string BornYear { get; set; }
    public string DiedYear { get; set; }
    public string Style { get; set; }
    public string Genre { get; set; }
    public string Website { get; set; }
    public string Facebook { get; set; }
    public string Twitter { get; set; }
}
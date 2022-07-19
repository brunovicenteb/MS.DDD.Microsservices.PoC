using Toolkit.Data;

namespace Benefit.Domain.Benefit;

public class TheAudioDbWork : BaseEntity
{
    public TheAudioDbWork()
    {
    }

    public TheAudioDbWork(string idArtist, string artist, string artistAlternate, string label,
        string formedYear, string bornYear, string diedYear, string style, string genre, string website,
        string facebook, string twitter, int beneficiaryID)
    {
        IdArtist = idArtist;
        Artist = artist;
        ArtistAlternate = artistAlternate;
        Label = label;
        FormedYear = formedYear;
        BornYear = bornYear;
        DiedYear = diedYear;
        Style = style;
        Genre = genre;
        Website = website;
        Facebook = facebook;
        Twitter = twitter;
        BeneficiaryID = beneficiaryID;
    }

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
    public int BeneficiaryID { get; set; }
    public virtual Beneficiary Beneficiary { get; set; }
}
using Toolkit.Data;

namespace Benefit.Domain.Benefit;

public class ImdbWork : BaseEntity
{
    public ImdbWork()
    {
    }

    public ImdbWork(string title, string image, string description, int beneficiaryID)
    {
        Title = title;
        Image = image;
        Description = description;
        BeneficiaryID = beneficiaryID;
    }

    public string Title { get; set; }
    public string Image { get; set; }
    public string Description { get; set; }
    public int BeneficiaryID { get; set; }
    public virtual Beneficiary Beneficiary { get; set; }
}
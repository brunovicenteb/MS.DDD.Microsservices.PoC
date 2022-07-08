namespace Benefit.Domain.Benefit;

public class Work
{
    public Work()
    {
    }

    public Work(string title, string image, string description)
    {
        Title = title;
        Image = image;
        Description = description;
    }

    public string Title { get; set; }
    public string Image { get; set; }
    public string Description { get; set; }
}
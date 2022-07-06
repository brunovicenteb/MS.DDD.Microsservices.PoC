namespace Benefit.Service.Model;

public class ImdbPerson
{
    public string searchType { get; set; }
    public string expression { get; set; }
    public List<ImdbResult> results { get; set; }
    public string errorMessage { get; set; }
}

public class ImdbResult
{
    public string id { get; set; }
    public string resultType { get; set; }
    public string image { get; set; }
    public string title { get; set; }
    public string description { get; set; }
}
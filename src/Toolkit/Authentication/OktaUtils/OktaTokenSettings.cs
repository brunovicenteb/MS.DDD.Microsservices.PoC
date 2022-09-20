namespace Toolkit.Authentication.OktaUtils;

public class OktaTokenSettings
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Domain { get; set; }
    public string AutorizationServerId { get; set; }
    public string Audience { get; set; }
    public string ApiKey { get; set; }
}
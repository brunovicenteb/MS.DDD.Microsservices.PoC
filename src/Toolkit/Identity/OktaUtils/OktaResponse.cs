using Newtonsoft.Json;

namespace Toolkit.Identity.OktaUtils;

public class OktaResponse
{
    [JsonProperty(PropertyName = "access_token")]
    public string AccessToken { get; set; }

    [JsonProperty(PropertyName = "expires_in")]
    public int ExpiresIn { get; set; }

    [JsonProperty(PropertyName = "token_type")]
    public string TokenType { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Scope { get; set; }

    public bool IsValidAndNotExpiring
    {
        get
        {
            return !string.IsNullOrEmpty(AccessToken) && ExpiresAt > DateTime.UtcNow.AddSeconds(30);
        }
    }
}
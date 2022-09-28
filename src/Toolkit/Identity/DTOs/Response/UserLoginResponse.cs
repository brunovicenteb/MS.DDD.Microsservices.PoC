using System.Text.Json.Serialization;

namespace Toolkit.Identity.DTOs.Response;

public class UserLoginResponse
{
    public bool Sucess => 
        Errors.Count == 0;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string AccessToken { get; private set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string RefreshToken { get; private set; }

    public List<string> Errors { get; private set; }

    public UserLoginResponse() =>
        Errors = new List<string>();

    public UserLoginResponse(string accessToken, string refreshToken) : this()
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public void AddError(string error) => 
        Errors.Add(error);

    public void AddError(IEnumerable<string> errors) =>
        Errors.AddRange(errors);
}
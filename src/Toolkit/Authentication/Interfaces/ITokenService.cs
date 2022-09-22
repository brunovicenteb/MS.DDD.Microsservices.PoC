using Toolkit.Authentication.OktaUtils;

namespace Toolkit.Authentication.Interfaces;

public interface ITokenService
{
    //Task<OktaResponse> GetToken();
    Task<OktaResponse> GetToken(string username, string password);
}
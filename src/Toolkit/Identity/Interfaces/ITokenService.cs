using Toolkit.Identity.OktaUtils;

namespace Toolkit.Identity.Interfaces;

public interface ITokenService
{
    //Task<OktaResponse> GetToken();
    Task<OktaResponse> GetToken(string username, string password);
}
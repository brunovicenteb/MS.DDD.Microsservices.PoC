namespace Toolkit.OutBox;

public interface IAuthenticable
{
    public void UseIdentity(string issuerVarName = "IDENTITY_ISSUER", string audienceVarName = "IDENTITY_AUDIENCE",
       string securityKeyVarName = "IDENTITY_SECURITY_KEY", string algorithmVarName = "IDENTITY_ALGORITHM",
        string accessTokenExpirationVarName = "IDENTITY_ACCESS_TOKEN_EXPIRATION",
       string refreshTokenExpirationVarName = "IDENTITY_REFRESH_TOKEN_EXPIRATION");

    public void UseOkta(string clientIdVarName = "OKTA_CLIENT_ID", string clienteSecretVarName = "OKTA_SECRET",
        string domainVarName = "OKTA_DOMAIN", string autorizationServerIdVarName = "OKTA_SERVER_ID",
        string audienceVarName = "OKTA_AUDIENCE");
    public void NotUseAuthentication();
}
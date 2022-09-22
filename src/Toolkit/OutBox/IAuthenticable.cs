namespace Toolkit.OutBox;

public interface IAuthenticable
{
    public void UseOkta(string clientIdVarName = "OKTA_CLIENT_ID", string clienteSecretVarName = "OKTA_SECRET",
        string domainVarName = "OKTA_DOMAIN", string autorizationServerIdVarName = "OKTA_SERVER_ID", 
        string audienceVarName = "OKTA_AUDIENCE");
    public void NotUseAuthentication();
}
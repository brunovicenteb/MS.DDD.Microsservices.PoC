using Okta.Sdk;
using Toolkit.Identity.DTOs.Request;
using Toolkit.Identity.DTOs.Response;

namespace Toolkit.Identity.Interfaces;

public interface IIdentityService
{
    Task<UserCreateResponse> CreateUser(UserCreateRequest user);
    Task<UserLoginResponse> Login(UserLoginRequest userLogin);
}
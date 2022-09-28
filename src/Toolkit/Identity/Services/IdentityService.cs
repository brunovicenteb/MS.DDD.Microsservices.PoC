using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Toolkit.Identity.Interfaces;
using Toolkit.Identity.DTOs.Response;
using Toolkit.Identity.DTOs.Request;

namespace Toolkit.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly SignInManager<IdentityUser> _SignInManager;
        private readonly UserManager<IdentityUser> _UserManager;
        //private readonly JwtOptions _JwtOptions;

        public IdentityService(SignInManager<IdentityUser> signInManager,
                               UserManager<IdentityUser> userManager)
        //IOptions<JwtOptions> jwtOptions)
        {
            _SignInManager = signInManager;
            _UserManager = userManager;
            //_JwtOptions = jwtOptions.Value;
        }

        public async Task<UserCreateResponse> CreateUser(UserCreateRequest user)
        {
            var identityUser = new IdentityUser
            {
                UserName = user.Email,
                Email = user.Email,
                EmailConfirmed = true
            };

            var result = await _UserManager.CreateAsync(identityUser, user.Password);
            if (result.Succeeded)
                await _UserManager.SetLockoutEnabledAsync(identityUser, false);

            var usuarioCadastroResponse = new UserCreateResponse(result.Succeeded);
            if (!result.Succeeded && result.Errors.Count() > 0)
                usuarioCadastroResponse.AddErrors(result.Errors.Select(r => r.Description));

            return usuarioCadastroResponse;
        }

        public async Task<UserLoginResponse> Login(UserLoginRequest userLogin)
        {
            var result = await _SignInManager.PasswordSignInAsync(userLogin.Email, userLogin.Senha, false, true);
            if (result.Succeeded)
                return await GenerateCredentials(userLogin.Email);

            var response = new UserLoginResponse();
            if (!result.Succeeded)
            {
                var error = "Username or password is incorrect";
                if (result.IsLockedOut)
                    error = "Blocked account";
                else if (result.IsNotAllowed)
                    error = "This account does not have permission to login";
                else if (result.RequiresTwoFactor)
                    error = "You need to confirm login in your second factor of authentication";
                response.AddError(error);
            }
            return response;
        }

        private async Task<UserLoginResponse> GenerateCredentials(string email)
        {
            var user = await _UserManager.FindByEmailAsync(email);
            var accessTokenClaims = await GetClaims(user, adicionarClaimsUsuario: true);
            var refreshTokenClaims = await GetClaims(user, adicionarClaimsUsuario: false);

            //var dataExpiracaoAccessToken = DateTime.Now.AddSeconds(_JwtOptions.AccessTokenExpiration);
            //var dataExpiracaoRefreshToken = DateTime.Now.AddSeconds(_JwtOptions.RefreshTokenExpiration);

            //var accessToken = GerarToken(accessTokenClaims, dataExpiracaoAccessToken);
            //var refreshToken = GerarToken(refreshTokenClaims, dataExpiracaoRefreshToken);

            return new UserLoginResponse("", "");
            //(
            //    Sucess: true,
            //    AccessToken: accessToken,
            //    refreshToken: refreshToken
            //);
        }

        private string GenerateToken(IEnumerable<Claim> claims, DateTime dataExpiracao)
        {
            //var jwt = new JwtSecurityToken(
            //    issuer: _JwtOptions.Issuer,
            //    audience: _JwtOptions.Audience,
            //    claims: claims,
            //    notBefore: DateTime.Now,
            //    expires: dataExpiracao,
            //    signingCredentials: _JwtOptions.SigningCredentials);

            //return new JwtSecurityTokenHandler().WriteToken(jwt);
            return string.Empty;
        }

        private async Task<IList<Claim>> GetClaims(IdentityUser user, bool adicionarClaimsUsuario)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()));

            if (adicionarClaimsUsuario)
            {
                var userClaims = await _UserManager.GetClaimsAsync(user);
                var roles = await _UserManager.GetRolesAsync(user);
                claims.AddRange(userClaims);
                foreach (var role in roles)
                    claims.Add(new Claim("role", role));
            }
            return claims;
        }
    }
}
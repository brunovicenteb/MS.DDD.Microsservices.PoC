using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Okta.AspNetCore;
using Toolkit.Authentication.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Toolkit.Authentication.OktaUtils;

public class OktaBuilder
{
    public static OktaTokenSettings TokenSettings { get; private set; }
    private const string _Bearer = "Bearer";

    public static void AddSecurityDefinition(IServiceCollection services, string clientId, string clientSecret, string domain,
       string autorizationServerId, string audience)
    {
        ConfigureSwagger(services);
        CreateConfiguration(services, clientId, clientSecret, domain, autorizationServerId, audience);
        services.AddCors(options =>
        {
            // The CORS policy is open for testing purposes. In a production application, you should restrict it to known origins.
            options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin()
                                                        .AllowAnyMethod()
                                                        .AllowAnyHeader());
        });
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = OktaDefaults.ApiAuthenticationScheme;
            options.DefaultChallengeScheme = OktaDefaults.ApiAuthenticationScheme;
            options.DefaultSignInScheme = OktaDefaults.ApiAuthenticationScheme;
        })
        .AddOktaWebApi(new OktaWebApiOptions
        {
            OktaDomain = domain,
            AuthorizationServerId = autorizationServerId,
            //Audience = audience
        });
        services.AddAuthorization();
        services.AddControllers();
        services.AddSingleton<ITokenService, OktaTokenService>();
    }

    private static void CreateConfiguration(IServiceCollection services, string clientId,
        string clientSecret, string domain, string autorizationServerId, string audience)
    {
        TokenSettings = new OktaTokenSettings()
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            Domain = domain,
            AutorizationServerId = autorizationServerId,
            Audience = audience
        };
        services.Configure<OktaTokenSettings>(c =>
        {
            c.ClientId = clientId;
            c.ClientSecret = clientSecret;
            c.Domain = domain;
            c.AutorizationServerId = autorizationServerId;
            c.Audience = audience;
        });
    }

    private static void ConfigureSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition(_Bearer, new OpenApiSecurityScheme
            {
                Description = "Okta Authorization header using the Bearer scheme. \"Autorization: Bearer <token>",
                Name = "Autorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Name = _Bearer,
                        Type = SecuritySchemeType.ApiKey,
                        In = ParameterLocation.Header,
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = _Bearer
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}
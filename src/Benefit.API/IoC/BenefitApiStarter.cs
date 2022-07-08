using System.Reflection;
using Benefit.Domain.Interfaces;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Toolkit.MessageBroker;
using Benefit.Service.Infra;
using Toolkit.Mongo;

namespace Benefit.API.IoC;

public static class StartBenefitApi
{
    public static IServiceCollection ConfigBenefitApi(this IServiceCollection services)
    {
        services.AddControllers()
            .AddNewtonsoftJson(o => o.SerializerSettings.Converters.Add(new StringEnumConverter()));
        services.AddSwaggerGenNewtonsoftSupport();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", CreateApiInfo());
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            opt.IncludeXmlComments(xmlPath);
            xmlFile = $"Benefit.Domain.xml";
            xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            opt.IncludeXmlComments(xmlPath);
        });
        services.AddProducers();
        services.AddMongoDb();
        services.AddScoped<IBenefitRepository, BenefitRepository>();
        return services;
    }

    public static void UseBenefitApi(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Benefit.API v1"));
    }

    private static OpenApiInfo CreateApiInfo()
    {
        return new OpenApiInfo
        {
            Title = "Chamados-Service API",
            Version = "v1",
            Description = "Essa é uma Api de fins didáticos, criada validar um conceito minimalista de arquitetura pra microsserviços. " +
                   "Seu intuito é apresentar uma arquitetura minimalista e ser o embrião para a discussão junto com a equipe.",
            Contact = new OpenApiContact
            {
                Name = "Bruno Belchior",
                Email = "brunovicenteb@gmail.com",
                Url = new Uri("https://github.com/brunovicenteb")
            },
            License = new OpenApiLicense
            {
                Name = "GPL-3.0",
                Url = new Uri("https://opensource.org/licenses/GPL-3.0")
            },
            TermsOfService = new Uri("https://opensource.org/licenses/GPL-3.0")
        };
    }
}
using Toolkit.Identity.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Toolkit.Identity.Services
{
    public static class IdentityServiceBuild
    {
        public static void AddSecurityDefinition(this IServiceCollection services)
        {
            //services.AddDbContext<DataContext>(options =>
            //    options.UseSqlServer(configuration.GetConnectionString("Way2CommerceConnection"))
            //);

            //services.AddDbContext<IdentityDataContext>(options =>
            //    options.UseSqlServer(configuration.GetConnectionString("Way2CommerceConnection"))
            //);

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                //.AddEntityFrameworkStores<IdentityDataContext>()
                .AddDefaultTokenProviders();
            services.AddScoped<IIdentityService, IdentityService>();
        }
    }
}
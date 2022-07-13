using Serilog;
using Benefit.API.IoC;
using Benefit.Service.IoC;
using Benefit.Service.Services;
using Benefit.Service.Interfaces;
using Toolkit.TransactionalOutBox;

var builder = WebApplication.CreateBuilder(args);

builder.UseTransactionalOutBox<BenefitContext>()
    .UseSerilog()
    .DoNotOpenTelemetry()
    .UseSqlServer(true)
    .UseRabbitMq();

builder.Services.AddScoped<IBeneficiaryService, BeneficiaryService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigBenefitApi();

var app = builder.Build();

app.UseExceptionHandler("/error");
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
app.UseRouting();
app.UseBenefitApi();
app.MapControllers();

Log.Logger.Information("#############################################################");
Log.Logger.Information("###                    Running Configs                    ###");
Log.Logger.Information("#############################################################");

app.Run();
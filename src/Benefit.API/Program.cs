using Benefit.API.IoC;
using Benefit.Service.IoC;
using Toolkit.OutBox;
using Toolkit.TransactionalOutBox;

var builder = WebApplication.CreateBuilder(args);

builder.UseTransactionalOutBox<BenefitContext>()
    .UseSerilog()
    .DoNotOpenTelemetry()
    .UseDatabase(DatabaseType.SqlServer)
    .UseRabbitMq();

builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigBenefitApi();

var app = builder.Build();

app.UseExceptionHandler("/error");
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
app.UseRouting();
app.UseBenefitApi();
app.MapControllers();

app.Run();
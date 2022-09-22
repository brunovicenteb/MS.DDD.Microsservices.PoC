using Toolkit.OutBox;
using Benefit.API.IoC;
using Benefit.Service.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.BeginProducer<BenefitContext>()
    .UseSerilog()
    .UseTelemetry()
    .UseDatabase()
    .UseRabbitMq()
    .UseOkta();

builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigBenefitApi();

var app = builder.Build();

app.UseExceptionHandler("/error");
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
app.UseRouting();
app.UseBenefitApi();
app.MapControllers();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.Run();
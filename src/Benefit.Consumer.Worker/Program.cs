using Toolkit.OutBox;
using Benefit.Service.IoC;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.BeginConsumer<BenefitContext>()
    .UseSerilog()
    .UseOpenTelemetry()
    .UseDatabase()
    .UseRabbitMq();

var app = builder.Build();
app.Run();
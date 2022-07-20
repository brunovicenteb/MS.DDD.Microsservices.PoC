using Toolkit.OutBox;
using Benefit.Service.IoC;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.BeginConsumer<BenefitContext>()
    .UseSerilog()
    .DoNotOpenTelemetry()
    .UseDatabase()
    .UseRabbitMq();

var app = builder.Build();
app.Run();
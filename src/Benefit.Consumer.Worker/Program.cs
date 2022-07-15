using Toolkit.OutBox;
using Benefit.Service.IoC;
using Toolkit.TransactionalOutBox;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.BeginConsumer<BenefitContext>(DatabaseType.Postgress)
    .UseSerilog()
    .DoNotOpenTelemetry()
    .UseDatabase()
    .UseRabbitMq();

var app = builder.Build();
app.Run();
using Microsoft.AspNetCore.Builder;
using Toolkit.MessageBroker;
using Benefit.Service.Workers;
using Toolkit.Mongo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMongoDb("BenefitMongoConnection", "BenefitMongoDb");
builder.Services.AddConsumers<BenefitConsumerFactory>();

var app = builder.Build();
app.Run();
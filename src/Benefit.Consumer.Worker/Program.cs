using Toolkit.Mongo;
using Toolkit.MessageBroker;
using Benefit.Service.Workers;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMongoDb();
builder.Services.AddConsumers<BenefitConsumerFactory>();

var app = builder.Build();
app.Run();
using Microsoft.AspNetCore.Builder;
using Toolkit.MessageBroker;
using Benefit.Service.Workers;
using Toolkit.Mongo;

var builder = WebApplication.CreateBuilder(args);

var host = Host.CreateDefaultBuilder(args)
    .AddMongoDb("BenefitMongoConnection", "BenefitMongoDb")
    .AddConsumers<BenefitConsumerFactory>()
    .Build();

host.Run();
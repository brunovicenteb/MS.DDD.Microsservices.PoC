//using MassTransit;
//using Benefit.Consumer.Worker.Workers;

//namespace Benefit.Consumer.Worker
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            CreateHostBuilder(args).Build().Run();
//        }

//        public static IHostBuilder CreateHostBuilder(string[] args) =>
//            Host.CreateDefaultBuilder(args)
//                .ConfigureServices((hostContext, services) =>
//                {
//                    services.AddMassTransit(x =>
//                    {
//                        x.AddConsumer<BenefitInsertedConsumer>();
//                        x.UsingRabbitMq((context, cfg) =>
//                        {
//                            cfg.ConfigureEndpoints(context);
//                        });
//                    });
//                });
//    }
//}


using Serilog;
using Microsoft.AspNetCore.Builder;
using Benefit.Consumer.Worker.Workers;
using Toolkit.MessageBroker;

var builder = WebApplication.CreateBuilder(args);

var host = Host.CreateDefaultBuilder(args)
    //.UseSerilog(Log.Logger)
    .AddConsumers<BenefitConsumerFactory>()
    .Build();

host.Run();
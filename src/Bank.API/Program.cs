using Serilog;
using Benefit.API.IoC;
using AutoMapper;
using Benefit.API.Mapping;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.ConfigBenefitApi();
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);

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
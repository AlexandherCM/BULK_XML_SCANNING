using BULK_DOWNLOAD_CFDI.PaternDesign;
using BULK_DOWNLOAD_CFDI.ToolsAndExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.ConfigureAppSettings();
    })
    .ConfigureServices((context, services) =>
    {
        services.RegisterServices(context.Configuration);
    }).Build();

//CREAR SERVICIO SCOPE
using var scope = host.Services.CreateScope();
var service = scope.ServiceProvider.GetRequiredService<Mediator>();

//EJECUTAR VOLCADO
await service.DumpCFDIs();
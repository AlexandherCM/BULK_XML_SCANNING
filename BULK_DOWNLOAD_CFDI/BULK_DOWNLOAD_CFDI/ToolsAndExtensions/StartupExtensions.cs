using BULK_DOWNLOAD_CFDI.Models;
using BULK_DOWNLOAD_CFDI.PaternDesign;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#pragma warning disable CS8600


namespace BULK_DOWNLOAD_CFDI.ToolsAndExtensions
{
    public static class StartupExtensions
    {
        public static void ConfigureAppSettings(this IConfigurationBuilder config)
        {
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        }

        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            // ContextJCJFDATA
            string connectionString = configuration.GetConnectionString("Context");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("La cadena de conexión no está configurada.");

            //IYECCIÓN DE LA DEPENDENCIA DE JCJFDATA
            services.AddDbContext<Context>(options =>
            {
                options
                    .UseSqlServer(connectionString)
                    .UseLoggerFactory(LoggerFactory.Create(builder => { })) // sin logs
                    .EnableSensitiveDataLogging(false);
            }, ServiceLifetime.Scoped);

            services.AddSingleton(configuration);
            services.AddScoped<Mediator>();
        }
    }
}

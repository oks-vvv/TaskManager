using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TaskManager.Console.Settings;
using TaskManager.Settings;

namespace TaskManager
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            await new HostBuilder()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", false);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    ConfigureServices(services);
                })
                .RunConsoleAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // settings
            static T AddSetting<T>(IServiceProvider svc)
                where T : new()
                => svc.GetRequiredService<IConfiguration>().GetSection(typeof(T).Name).Get<T>() ?? new T();

            services.AddSingleton(AddSetting<ClientSettings>);
            services.AddSingleton(AddSetting<ProcessSettings>);

            // process
            // services.AddTransient<IFileParser, FileParser>();

        }
    }
}

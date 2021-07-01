using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using TaskManager.Settings;
using TaskManager.Contracts;
using TaskManager.Process;
using TaskManager.TaskManager;
using TaskManager.Contracts.Validation;
using System.Configuration;
using Microsoft.Extensions.Hosting;

namespace TaskManager.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                opts.JsonSerializerOptions.IgnoreNullValues = true;
            }); ;

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskManagerApi", Version = "v1" }); });

            ConfigureAppServices(services);
        }

        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime, ITaskManager taskManager)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManagerApi - v1"));

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            applicationLifetime.ApplicationStopping.Register(() => { taskManager.Dispose(); });
        }

        private static void ConfigureAppServices(IServiceCollection services)
        {
            // settings
            static T AddSetting<T>(IServiceProvider svc)
                where T : IValidated, new()
            {
                var setting = svc.GetRequiredService<IConfiguration>().GetSection(typeof(T).Name).Get<T>() ?? new T();
                var result = setting.Validate();
                if (!result.IsValid)
                {
                    throw new ConfigurationErrorsException(result.FailureMessage);
                }

                return setting;
            }

            services.AddSingleton(AddSetting<AppSettings>);
            services.AddSingleton(AddSetting<ProcessSettings>);

            // tasks
            services.AddSingleton<ITaskStore, InMemoryTaskStore>();
            services.AddSingleton<ITaskManager>(svc =>
            {
                var settings = svc.GetRequiredService<AppSettings>();
                var processService = svc.GetRequiredService<IProcessService>();
                var taskStore = svc.GetRequiredService<ITaskStore>();
                return settings.AddProcessStrategy switch
                {
                    AddProcessStrategy.Default => new DefaultTaskManager(processService, taskStore, settings.Capacity),
                    AddProcessStrategy.FifoAdd => new FifoTaskManager(processService, taskStore, settings.Capacity),
                    AddProcessStrategy.PriorityFifoAdd => new PriorityTaskManager(processService, taskStore, settings.Capacity),
                    _ => throw new NotSupportedException($"{settings.AddProcessStrategy} not supported"),
                };
            });

            services.AddTransient<IProcessService, ProcessService>();
        }
    }
}

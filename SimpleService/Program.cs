using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleComponents.Consumers;
using SimpleService.OptionsModel;
using System.Diagnostics;

namespace SimpleService
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            var builder = new HostBuilder()
                .ConfigureAppConfiguration((cont, config) =>
                {
                    config.AddJsonFile("appsettings.json", true);
                    config.AddEnvironmentVariables();

                    if (args is not null)
                        config.AddCommandLine(args);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddOptions<RabbitMQConfig>().Bind(context.Configuration.GetSection("RabbitMQConfig"));

                    services.AddMassTransit(cfg =>
                    {
                        cfg.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

                        cfg.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();

                        cfg.UsingRabbitMq((context, configurator) =>
                        {
                            //var rabbit = context.GetService<IOptions<RabbitMQConfig>>().Value;

                            configurator.ConfigureEndpoints(context);
                        });
                    });                    

                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });

            if (isService)
                await builder.UseWindowsService().Build().RunAsync();
            else
                await builder.RunConsoleAsync();
           

            #region My first custom Host Builder
            //var host = Host.CreateDefaultBuilder(args)
            //    .ConfigureLogging((_, logging) =>
            //    {
            //        logging.ClearProviders();
            //        logging.AddDebug();
            //    })
            //    .ConfigureServices((ctx, services) =>
            //    {
            //        //services.AddHostedService<MyService>();

            //        services.AddSingleton<MyApp>();
            //    })
            //    .Build();

            //var app = host.Services.GetRequiredService<MyApp>();

            //var result = await app.StartAsync();

            ////host.Start();
            //Console.WriteLine("Done");
            ////host.StopAsync().Wait();

            //Console.ReadLine();
            //return result;

            #endregion
        }

        public static IBusControl ConfigureBus(IBusRegistrationContext serviceProvider)
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.ConfigureEndpoints(serviceProvider);
            });
        }
    }

    

    //public class MyService : IHostedService
    //{
    //    public Task StartAsync(CancellationToken cancellationToken)
    //    {
    //        Console.WriteLine("Starting");
    //        return Task.CompletedTask;
    //    }

    //    public Task StopAsync(CancellationToken cancellationToken)
    //    {
    //        Console.WriteLine("Stopping");
    //        return Task.CompletedTask;
    //    }
    //}
}

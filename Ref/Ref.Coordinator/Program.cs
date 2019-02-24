using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ref.Coordinator.Core;
using Ref.Coordinator.DI;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ref.Coordinator
{
    class Program
    {
        private static readonly string appId = "coordinator";

        static async Task<int> Main(string[] args)
        {
            NLog.LogManager.Configuration.Variables["fileName"] = $"ref-{appId}-{DateTime.UtcNow.ToString("ddMMyyyy")}.log";
            NLog.LogManager.Configuration.Variables["archiveFileName"] = $"ref-{appId}-{DateTime.UtcNow.ToString("ddMMyyyy")}.log";

            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"appsettings.{appId}.json");

            var configuration = builder.Build();

            var servicesProvider = DependencyProvider.Get(configuration, appId);

            await servicesProvider.GetRequiredService<CoordinatorService>().Manage();

            NLog.LogManager.Shutdown();

            return 0;
        }
    }
}
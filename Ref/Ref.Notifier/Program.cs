using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ref.Notifier.Core;
using Ref.Notifier.DI;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ref.Notifier
{
    class Program
    {
        private static readonly string appId = "notifier";

        static async Task<int> Main(string[] args)
        {
            NLog.LogManager.Configuration.Variables["fileName"] = $"ref-{appId}-{DateTime.UtcNow.ToString("ddMMyyyy")}.log";
            NLog.LogManager.Configuration.Variables["archiveFileName"] = $"ref-{appId}-{DateTime.UtcNow.ToString("ddMMyyyy")}.log";

            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"appsettings.{appId}.json");

            var configuration = builder.Build();

            var servicesProvider = DependencyProvider.Get(configuration, appId);

            await servicesProvider.GetRequiredService<NotifierService>().Notify();

            NLog.LogManager.Shutdown();

            return 0;
        }
    }
}
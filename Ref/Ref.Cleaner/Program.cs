using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ref.Cleaner.Core;
using Ref.Cleaner.DI;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ref.Cleaner
{
    class Program
    {
        private static readonly string appId = "cleaner";

        static async Task<int> Main(string[] args)
        {
            NLog.LogManager.Configuration.Variables["fileName"] = $"ref-{appId}-{DateTime.UtcNow.ToString("ddMMyyyy")}.log";
            NLog.LogManager.Configuration.Variables["archiveFileName"] = $"ref-{appId}-{DateTime.UtcNow.ToString("ddMMyyyy")}.log";

            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"appsettings.{appId}.json");

            var configuration = builder.Build();

            var servicesProvider = DependencyProvider.Get(configuration, appId);

            await servicesProvider.GetRequiredService<CleanService>().Clean();

            NLog.LogManager.Shutdown();

            return 0;
        }
    }
}
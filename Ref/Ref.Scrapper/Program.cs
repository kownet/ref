using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ref.Scrapper.Core;
using Ref.Scrapper.DI;
using Ref.Shared.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ref.Scrapper
{
    class Program
    {
        private static string appId = "";

        static async Task<int> Main(string[] args)
        {
            if (args.AnyAndNotNull())
            {
                appId = $"{args[0]}";

                NLog.LogManager.Configuration.Variables["fileName"] = $"ref-{appId}-{DateTime.UtcNow.ToString("ddMMyyyy")}.log";
                NLog.LogManager.Configuration.Variables["archiveFileName"] = $"ref-{appId}-{DateTime.UtcNow.ToString("ddMMyyyy")}.log";

                var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile($"appsettings.{appId}.json");

                var configuration = builder.Build();

                var servicesProvider = DependencyProvider.Get(configuration, appId);

                await servicesProvider.GetRequiredService<ScrapperService>().Scrapp();

                NLog.LogManager.Shutdown();
            }

            return 0;
        }
    }
}
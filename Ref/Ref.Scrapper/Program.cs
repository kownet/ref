using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ref.Scrapper.Core;
using Ref.Scrapper.DI;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ref.Scrapper
{
    class Program
    {
        private static readonly string appId = "scrapper";

        static async Task<int> Main(string[] args)
        {
            NLog.LogManager.Configuration.Variables["fileName"] = $"ref-{appId}-{DateTime.UtcNow.ToString("ddMMyyyy")}.log";
            NLog.LogManager.Configuration.Variables["archiveFileName"] = $"ref-{appId}-{DateTime.UtcNow.ToString("ddMMyyyy")}.log";

            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"appsettings.{appId}.json");

            var configuration = builder.Build();

            var servicesProvider = DependencyProvider.Get(configuration, appId);

            await servicesProvider.GetRequiredService<ScrapperService>().Scrapp();

            NLog.LogManager.Shutdown();

            return 0;
        }
    }
}
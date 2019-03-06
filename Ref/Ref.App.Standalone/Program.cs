using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ref.App.Standalone.Core;
using Ref.App.Standalone.DI;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Ref.App.Standalone
{
    class Program
    {
        private static readonly string appId = "standalone";

        static async Task<int> Main(string[] args)
        {
            EncodingProvider provider = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(provider);

            NLog.LogManager.Configuration.Variables["fileName"] = $"ref-{appId}-{DateTime.UtcNow.ToString("ddMMyyyy")}.log";
            NLog.LogManager.Configuration.Variables["archiveFileName"] = $"ref-{appId}-{DateTime.UtcNow.ToString("ddMMyyyy")}.log";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{appId}.json");

            var configuration = builder.Build();

            var servicesProvider = DependencyProvider.Get(configuration, appId);

            await servicesProvider.GetRequiredService<RefStandaloneService>().Crawl();

            NLog.LogManager.Shutdown();

            return 0;
        }
    }
}
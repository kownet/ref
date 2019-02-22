using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ref.App.Core;
using Ref.App.DI;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Ref.App
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                var appId = args[0];

                EncodingProvider provider = CodePagesEncodingProvider.Instance;
                Encoding.RegisterProvider(provider);

                NLog.LogManager.Configuration.Variables["fileName"] = $"ref-{appId}-{DateTime.UtcNow.ToString("ddMMyyyy")}.log";
                NLog.LogManager.Configuration.Variables["archiveFileName"] = $"ref-{appId}-{DateTime.UtcNow.ToString("ddMMyyyy")}.log";

                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"appsettings.{appId}.json");

                var configuration = builder.Build();

                var servicesProvider = DependencyProvider.Get(configuration, appId);

                await servicesProvider.GetRequiredService<RefService>().Crawl();
            }

            NLog.LogManager.Shutdown();

            return 0;
        }
    }
}
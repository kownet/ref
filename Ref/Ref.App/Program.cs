﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ref.App.Core;
using Ref.App.DI;
using Ref.Shared.Providers;
using Ref.Shared.Utils;
using System;
using System.IO;

namespace Ref.App
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                var clientId = args[0];

                NLog.LogManager.Configuration.Variables["fileName"] = $"ref-{clientId}-{DateTime.UtcNow.ToString("ddMMyyyy")}.log";
                NLog.LogManager.Configuration.Variables["archiveFileName"] = $"ref-{clientId}-{DateTime.UtcNow.ToString("ddMMyyyy")}.log";

                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"appsettings.{clientId}.json");

                var configuration = builder.Build();

                var servicesProvider = DependencyProvider.Get(configuration, clientId);

                Create(
                    servicesProvider.GetService<IStorageProvider>(),
                    clientId);

                servicesProvider.GetRequiredService<RefService>().Crawl();
            }

            NLog.LogManager.Shutdown();
        }

        static void Create(IStorageProvider jsonStorageConfiguration, string clientId)
        {
            if (jsonStorageConfiguration != null)
            {
                var jsonStorageDir = Path.Combine(jsonStorageConfiguration.Dir());

                if (!Directory.Exists(jsonStorageDir))
                {
                    Directory.CreateDirectory(jsonStorageDir);
                }

                var jsonStorageFile = Path.Combine(jsonStorageConfiguration.Dir(), StorageFile.Name(clientId));

                if (!File.Exists(jsonStorageFile))
                {
                    File.Create(jsonStorageFile).Dispose();
                }
            }
        }
    }
}
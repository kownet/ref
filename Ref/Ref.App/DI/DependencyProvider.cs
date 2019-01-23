using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Ref.App.Core;
using Ref.Data.Repositories;
using Ref.Shared.Notifications;
using Ref.Shared.Providers;
using Ref.Sites;
using System;

namespace Ref.App.DI
{
    public static class DependencyProvider
    {
        public static IServiceProvider Get(IConfigurationRoot configurationRoot, string clientId)
        {
            var services = new ServiceCollection();

            #region Logging
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
            });
            #endregion

            #region Client
            services.AddTransient<IClientProvider>(s => new ClientProvider(clientId));
            #endregion

            #region Storages
            services.AddTransient<IStorageProvider>(
                s => new StorageProvider(
                    configurationRoot["storages:file_json:dir"],
                    clientId)
                );
            #endregion

            #region Notifications
            services.AddTransient<IPushOverProvider>(
                s => new PushOverProvider(
                    configurationRoot["notifications:pushover:token"],
                    configurationRoot["notifications:pushover:recipients"],
                    configurationRoot["notifications:pushover:endpoint"])
                );

            services.AddTransient<IPushOverNotification, PushOverNotification>();
            #endregion

            #region Repositories
            services.AddTransient<IAdRepository, JsonRepository>();
            #endregion

            #region Filters
            services.AddTransient<IFilterProvider>(
                s => new FilterProvider(
                        configurationRoot["filter:type"],
                        configurationRoot["filter:deal"],
                        configurationRoot["filter:location"],
                        configurationRoot["filter:flatareafrom"],
                        configurationRoot["filter:flatareato"],
                        configurationRoot["filter:pricefrom"],
                        configurationRoot["filter:priceto"],
                        configurationRoot["filter:market"],
                        configurationRoot["filter:newest"]
                    )
                );
            #endregion

            #region Sites
            services.AddTransient<ISite, OtoDomSite>();
            #endregion

            services.AddTransient<RefService>();

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
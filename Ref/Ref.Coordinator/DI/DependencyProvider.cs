using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Ref.Coordinator.Core;
using Ref.Data.Db;
using Ref.Data.Repositories;
using Ref.Shared.Notifications;
using Ref.Shared.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ref.Coordinator.DI
{
    public static class DependencyProvider
    {
        public static IServiceProvider Get(IConfigurationRoot configurationRoot, string appId)
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

            #region Storages
            services.AddTransient<IDbAccess>(
                s => new DbAccess(configurationRoot["storages:mssql:cs"])
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
            services.AddTransient<IFilterRepository, FilterRepository>();
            services.AddTransient<IOfferRepository, OfferRepository>();
            services.AddTransient<IOfferFilterRepository, OfferFilterRepository>();
            #endregion

            #region App
            services.AddTransient<IAppProvider>(
            s => new AppProvider(
                configurationRoot["app:address"],
                configurationRoot["app:sender"],
                configurationRoot["app:replyto"],
                configurationRoot["app:bcc"],
                configurationRoot["app:pausetime"],
                configurationRoot["app:timeout"],
                configurationRoot["app:sites"],
                appId,
                configurationRoot["app:adminnotification"],
                configurationRoot["app:successtries"],
                configurationRoot["app:mode"],
                configurationRoot["app:deals"])
            );
            #endregion

            services.AddTransient<CoordinatorService>();

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}